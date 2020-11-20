﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Mcma.Azure.CosmosDb
{
    public class CosmosDbTable : IDocumentDatabaseTable
    {
        public CosmosDbTable(ICustomQueryBuilderRegistry<(QueryDefinition, QueryRequestOptions)> customQueryBuilderRegistry,
                             IQueryDefinitionBuilder queryDefinitionBuilder,
                             ICosmosDbContainerProvider containerProvider,
                             IOptions<CosmosDbTableOptions> options)
        {
            CustomQueryBuilderRegistry = customQueryBuilderRegistry ?? throw new ArgumentNullException(nameof(customQueryBuilderRegistry));
            QueryDefinitionBuilder = queryDefinitionBuilder ?? throw new ArgumentNullException(nameof(queryDefinitionBuilder));
            ContainerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));
            Options = options.Value ?? new CosmosDbTableOptions();
        }

        private ICustomQueryBuilderRegistry<(QueryDefinition, QueryRequestOptions)> CustomQueryBuilderRegistry { get; }

        private IQueryDefinitionBuilder QueryDefinitionBuilder { get; }

        private ICosmosDbContainerProvider ContainerProvider { get; }

        private CosmosDbTableOptions Options { get; }

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
            var container = await ContainerProvider.GetAsync();

            queryRequestOptions.ConsistencyLevel ??= Options.ConsistentQuery.HasValue ? ConsistencyLevel.Strong : default;

            var queryIterator = container.GetItemQueryIterator<CosmosDbItem<T>>(queryDefinition, pageStartToken, queryRequestOptions);

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
            var containerProperties = await ContainerProvider.GetPropertiesAsync();
            var queryDefinition = QueryDefinitionBuilder.Build(query, containerProperties.PartitionKeyName());

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
            var container = await ContainerProvider.GetAsync();
            
            var resp =
                await container.ReadItemStreamAsync(Uri.EscapeDataString(id),
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
            var container = await ContainerProvider.GetAsync();
            
            var item = new CosmosDbItem<T>(id, resource);

            var resp = await container.UpsertItemAsync(item, ParsePartitionKey(id));

            return resp.Resource?.Resource;
        }

        public async Task DeleteAsync(string id)
        {
            var container = await ContainerProvider.GetAsync();
            var resp = await container.DeleteItemStreamAsync(Uri.EscapeDataString(id), ParsePartitionKey(id));
            resp.EnsureSuccessStatusCode();
        }

        public async Task<IDocumentDatabaseMutex> CreateMutexAsync(string mutexName, string mutexHolder, TimeSpan? lockTimeout = null)
        {
            var container = await ContainerProvider.GetAsync();
            var containerProperties = await ContainerProvider.GetPropertiesAsync();
            
            return new CosmosDbMutex(container, containerProperties.PartitionKeyName(), mutexName, mutexHolder, lockTimeout);   
        }
    }
}
