using Mcma.Data;
using Microsoft.Azure.Cosmos;

namespace Mcma.Azure.CosmosDb
{
    public class CosmosDbTableOptions : DocumentDatabaseTableOptions
    {
        public string Endpoint { get; set; } = McmaCosmosDbEnvironmentVariables.Endpoint;

        public string Key { get; set; } = McmaCosmosDbEnvironmentVariables.Key;

        public string DatabaseId { get; set; } = McmaCosmosDbEnvironmentVariables.DatabaseId;
        
        public bool? ConsistentGet { get; set; }
        
        public bool? ConsistentQuery { get; set; }

        public CosmosClientOptions CosmosClient { get; set; } = new CosmosClientOptions {ApplicationRegion = McmaCosmosDbEnvironmentVariables.Region};
    }
}
