using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mcma.Serialization;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace Mcma.Azure.CosmosDb
{
    public static class ResponseMessageExtensions
    {
        public static async Task<T> UnwrapResourceAsync<T>(this ResponseMessage responseMessage) where T : class
            => (await responseMessage.ToObjectAsync<CosmosDbItem<T>>())?.Resource;
        
        public static async Task<T> ToObjectAsync<T>(this ResponseMessage responseMessage) where T : class
        {
            string bodyText;
            using (var streamReader = new StreamReader(responseMessage.Content))
                bodyText = await streamReader.ReadToEndAsync();

            return McmaJson.Parse(bodyText).ToMcmaObject<T>();
        }
    }
    
    public class CosmosDbTable : IDocumentDatabaseTable
    {
        public CosmosDbTable(CosmosDbTableOptions options, Container container, ContainerProperties containerProperties)
        {
            Options = options;
            Container = container;

            var partitionKeyParts = containerProperties.PartitionKeyPath.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
            if (partitionKeyParts.Length > 1)
                throw new McmaException(
                    $"Container {containerProperties.Id} defines a partition key with multiple paths ({containerProperties.PartitionKeyPath}). MCMA only supports partition keys with a single path.");
            
            PartitionKeyName = partitionKeyParts[0];
        }

        private CosmosDbTableOptions Options { get; }

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
            var queryDefinition = CosmosDbQueryDefinitionBuilder.Build(query, PartitionKeyName);

            return await ExecuteQuery<T>(queryDefinition, new QueryRequestOptions { MaxItemCount = query.PageSize }, query.PageStartToken);
        }

        public async Task<QueryResults<TResource>> CustomQueryAsync<TResource, TParameters>(CustomQuery<TParameters> customQuery)
            where TResource : class
        {
            var buildCustomQuery = Options.GetCustomQueryBuilder<TParameters>(customQuery.Name);

            var (queryDefinition, queryRequestOptions) = buildCustomQuery(customQuery);

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
