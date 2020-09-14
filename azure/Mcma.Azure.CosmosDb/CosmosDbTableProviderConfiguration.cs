namespace Mcma.Azure.CosmosDb
{
    public class CosmosDbTableProviderConfiguration
    {
        public CosmosDbTableProviderConfiguration()
        {
        }

        public CosmosDbTableProviderConfiguration(string endpoint, string key, string region, string databaseId)
        {
            Endpoint = endpoint;
            Key = key;
            Region = region;
            DatabaseId = databaseId;
        }

        public string Endpoint { get; set; }

        public string Key { get; set; }

        public string Region { get; set; }

        public string DatabaseId { get; set; }
    }
}