using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Serialization;
using Mcma.Data;
using Microsoft.Azure.Cosmos;

namespace Mcma.Azure.CosmosDb
{
    public class CosmosDbTableProvider : IDocumentDatabaseTableProvider, IDisposable
    {
        public CosmosDbTableProvider(CosmosDbTableProviderConfiguration providerConfiguration = null, CosmosDbTableOptions tableOptions = null)
        {
            providerConfiguration ??= new CosmosDbTableProviderConfiguration().FromEnvironmentVariables(EnvironmentVariables.Instance);
            TableOptions = tableOptions ?? new CosmosDbTableOptions();

            CosmosClient =
                new CosmosClient(providerConfiguration.Endpoint,
                                 providerConfiguration.Key,
                                 new CosmosClientOptions
                                 {
                                     ApplicationRegion = providerConfiguration.Region,
                                     Serializer = new CosmosJsonDotNetSerializer(McmaJson.DefaultSettings())
                                 });

            Database = CosmosClient.GetDatabase(providerConfiguration.DatabaseId);
        }

        private CosmosDbTableOptions TableOptions { get; }

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
            
            return new CosmosDbTable(TableOptions, container, containerProperties);
        }

        public void Dispose() => CosmosClient?.Dispose();
    }
}
