using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Azure.Cosmos;

namespace Mcma.Azure.CosmosDb
{
    public class CosmosDbTable : IDocumentDatabaseTable
    {
        public CosmosDbTable(ICustomQueryBuilderRegistry<(QueryDefinition, QueryRequestOptions)> customQueryBuilderRegistry,
                             IQueryDefinitionBuilder queryDefinitionBuilder,
                             CosmosDbTableProviderOptions options,
                             Container container,
                             ContainerProperties containerProperties)
        {
            CustomQueryBuilderRegistry = customQueryBuilderRegistry ?? throw new ArgumentNullException(nameof(customQueryBuilderRegistry));
            QueryDefinitionBuilder = queryDefinitionBuilder ?? throw new ArgumentNullException(nameof(queryDefinitionBuilder));
            Container = container ?? throw new ArgumentNullException(nameof(container));
            
            if (containerProperties == null) throw new ArgumentNullException(nameof(containerProperties));

            var partitionKeyParts = containerProperties.PartitionKeyPath.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
            if (partitionKeyParts.Length > 1)
                throw new McmaException(
                    $"Container {containerProperties.Id} defines a partition key with multiple paths ({containerProperties.PartitionKeyPath}). MCMA only supports partition keys with a single path.");

            PartitionKeyName = partitionKeyParts[0];
            Options = options ?? new CosmosDbTableProviderOptions();
        }

        private ICustomQueryBuilderRegistry<(QueryDefinition, QueryRequestOptions)> CustomQueryBuilderRegistry { get; }

        private IQueryDefinitionBuilder QueryDefinitionBuilder { get; }

        private CosmosDbTableProviderOptions Options { get; }

        private Container Container { get; }
        
        private string PartitionKeyName { get; }

        private static PartitionKey ParsePartitionKey(string id)
        {
            var lastSlashIndex = id.LastIndexOf('/');
            return lastSlashIndex > 0 ? new PartitionKey(id.Substring(0, lastSlashIndex)) : PartitionKey.None;
        }
        
        private async Task<QueryResults<T>> ExecuteQuery<T>(QueryDefinition queryDefinition,
                                                            QueryRequestOptions queryRequestOptions,
                                                            string pageStartToken)
            where T : class
        {
            queryRequestOptions.ConsistencyLevel ??= Options.ConsistentQuery.HasValue ? ConsistencyLevel.Strong : default;

            var queryIterator = Container.GetItemQueryIterator<CosmosDbItem<T>>(queryDefinition, pageStartToken, queryRequestOptions);

            var results = new List<CosmosDbItem<T>>();
            
            string continuationToken;
            do
            {
                var response = await queryIterator.ReadNextAsync();

                results.AddRange(response);
                continuationToken = response.ContinuationToken;
            }
            while (queryIterator.HasMoreResults);

            return new QueryResults<T>
            {
                Results = results.Select(x => x.Resource).ToArray(),
                NextPageStartToken = continuationToken
            };
        }

        public async Task<QueryResults<T>> QueryAsync<T>(Query<T> query) where T : class
        {
            var queryDefinition = QueryDefinitionBuilder.Build(query, PartitionKeyName);

            return await ExecuteQuery<T>(queryDefinition, new QueryRequestOptions { MaxItemCount = query.PageSize }, query.PageStartToken);
        }

        public async Task<QueryResults<TResource>> CustomQueryAsync<TResource, TParameters>(CustomQuery<TParameters> customQuery)
            where TResource : class
        {
            var customQueryBuilder = CustomQueryBuilderRegistry.Get<TParameters>(customQuery.Name);

            var (queryDefinition, queryRequestOptions) = customQueryBuilder.Build(customQuery);

            return await ExecuteQuery<TResource>(queryDefinition, queryRequestOptions, customQuery.PageStartToken);
        }
        
        public async Task<T> GetAsync<T>(string id) where T : class
        {
            var resp =
                await Container.ReadItemStreamAsync(Uri.EscapeDataString(id),
                                                    ParsePartitionKey(id),
                                                    new ItemRequestOptions
                                                    {
                                                        ConsistencyLevel = Options.ConsistentGet.HasValue ? ConsistencyLevel.Strong : default
                                                    });

            if (resp.StatusCode == HttpStatusCode.NotFound)
                return default;

            resp.EnsureSuccessStatusCode();

            return await resp.UnwrapResourceAsync<T>();
        }

        public async Task<T> PutAsync<T>(string id, T resource) where T : class
        {
            var item = new CosmosDbItem<T>(id, resource);

            var resp = await Container.UpsertItemAsync(item, ParsePartitionKey(id));

            return resp.Resource?.Resource;
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await Container.DeleteItemStreamAsync(Uri.EscapeDataString(id), ParsePartitionKey(id));
            resp.EnsureSuccessStatusCode();
        }

        public IDocumentDatabaseMutex CreateMutex(string mutexName, string mutexHolder, TimeSpan? lockTimeout = null)
            => new CosmosDbMutex(Container, PartitionKeyName, mutexName, mutexHolder, lockTimeout);
    }
}
