using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Serialization;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Mcma.Azure.CosmosDb
{
    public class CosmosDbTableProvider : IDocumentDatabaseTableProvider, IDisposable
    {
        public CosmosDbTableProvider(ICustomQueryBuilderRegistry<(QueryDefinition, QueryRequestOptions)> customQueryBuilderRegistry,
                                     IQueryDefinitionBuilder queryDefinitionBuilder,
                                     IOptions<CosmosDbTableProviderOptions> options)
        {
            CustomQueryBuilderRegistry = customQueryBuilderRegistry ?? throw new ArgumentNullException(nameof(customQueryBuilderRegistry));
            QueryDefinitionBuilder = queryDefinitionBuilder ?? throw new ArgumentNullException(nameof(queryDefinitionBuilder));
            Options = options.Value ?? new CosmosDbTableProviderOptions();
            Options.CosmosClient.Serializer = new CosmosJsonDotNetSerializer(McmaJson.DefaultSettings());

            CosmosClient = new CosmosClient(Options.Endpoint, Options.Key, Options.CosmosClient);
            Database = CosmosClient.GetDatabase(Options.DatabaseId);
        }

        private ICustomQueryBuilderRegistry<(QueryDefinition, QueryRequestOptions)> CustomQueryBuilderRegistry { get; }

        private IQueryDefinitionBuilder QueryDefinitionBuilder { get; }

        private CosmosDbTableProviderOptions Options { get; }

        private CosmosClient CosmosClient { get; }
        
        private Database Database { get; }

        private Dictionary<string, (ContainerProperties containerProperties, Container container)> ContainerProperties { get; } =
            new Dictionary<string, (ContainerProperties containerProperties, Container container)>();
        
        private SemaphoreSlim ContainerPropertiesSemaphore { get; } = new SemaphoreSlim(1, 1);

        public async Task<IDocumentDatabaseTable> GetAsync(string tableName)
        {
            await ContainerPropertiesSemaphore.WaitAsync();
            
            ContainerProperties containerProperties;
            Container container;
            try
            {
                if (!ContainerProperties.ContainsKey(tableName))
                {
                    var resp = await Database.GetContainer(tableName).ReadContainerAsync();
                    ContainerProperties[tableName] = (resp.Resource, resp.Container);
                }

                (containerProperties, container) = ContainerProperties[tableName];
            }
            finally
            {
                ContainerPropertiesSemaphore.Release();
            }
            
            return new CosmosDbTable(CustomQueryBuilderRegistry, QueryDefinitionBuilder, Options, container, containerProperties);
        }

        public void Dispose() => CosmosClient?.Dispose();
    }
}
