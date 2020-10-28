using Mcma.Utility;

namespace Mcma.Azure.CosmosDb
{
    public static class McmaCosmosDbEnvironmentVariables
    {
        public const string EnvironmentVariablePrefix = "COSMOSDB_";

        public static string Get(string key) => McmaEnvironmentVariables.Get(EnvironmentVariablePrefix + key);
    }
}