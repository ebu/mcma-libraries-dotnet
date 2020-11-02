using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Mcma.Azure.CosmosDb
{
    internal class CosmosDbContainerProvider : ICosmosDbContainerProvider
    {
        public CosmosDbContainerProvider(IOptions<CosmosDbTableOptions> options)
        {
            Options = options?.Value ?? new CosmosDbTableOptions();

            CosmosClient = new CosmosClient(Options.Endpoint, Options.Key, Options.CosmosClient);
            Database = CosmosClient.GetDatabase(Options.DatabaseId);
        }
        
        private CosmosDbTableOptions Options { get; }

        private CosmosClient CosmosClient { get; }
        
        private Database Database { get; }
        
        private ContainerProperties ContainerProperties { get; set; }
        
        private Container Container { get; set; }
        
        private SemaphoreSlim ContainerPropertiesSemaphore { get; } = new SemaphoreSlim(1, 1);

        private async Task LoadAsync()
        {
            await ContainerPropertiesSemaphore.WaitAsync();
            try
            {
                if (Container != null)
                    return;
                
                var resp = await Database.GetContainer(Options.TableName).ReadContainerAsync();
                    
                ContainerProperties = resp.Resource;
                Container = resp.Container;
            }
            finally
            {
                ContainerPropertiesSemaphore.Release();
            }
        }

        public async Task<ContainerProperties> GetPropertiesAsync()
        {
            if (ContainerProperties == null)
                await LoadAsync();

            return ContainerProperties;
        }
        
        public async Task<Container> GetAsync()
        {
            if (Container == null)
                await LoadAsync();

            return Container;
        }
        
    }
}