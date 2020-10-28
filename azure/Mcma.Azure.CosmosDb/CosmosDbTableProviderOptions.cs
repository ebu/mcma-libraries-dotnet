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

        public static void SetFromEnvironmentVariables(CosmosDbTableProviderOptions options)
        {
            options.Endpoint = McmaCosmosDbEnvironmentVariables.Get("ENDPOINT");
            options.Key = McmaCosmosDbEnvironmentVariables.Get("KEY");
            options.DatabaseId = McmaCosmosDbEnvironmentVariables.Get("DATABASE_ID");
            options.CosmosClient.ApplicationRegion = McmaCosmosDbEnvironmentVariables.Get("REGION");
        }
    }
}
