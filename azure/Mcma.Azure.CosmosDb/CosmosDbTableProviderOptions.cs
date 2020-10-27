using Microsoft.Azure.Cosmos;

namespace Mcma.Azure.CosmosDb
{
    public class CosmosDbTableProviderOptions
    {
        public string Endpoint { get; set; }

        public string Key { get; set; }

        public string DatabaseId { get; set; }
        
        public bool? ConsistentGet { get; set; }
        
        public bool? ConsistentQuery { get; set; }
        
        public CosmosClientOptions CosmosClient { get; set; } = new CosmosClientOptions();
    }
}
