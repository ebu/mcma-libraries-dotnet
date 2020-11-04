using Mcma.Utility;

namespace Mcma.Azure.CosmosDb
{
    public static class McmaCosmosDbEnvironmentVariables
    {
        private const string EnvironmentVariablePrefix = "COSMOS_DB_";

        public static string Endpoint => McmaEnvironmentVariables.Get(EnvironmentVariablePrefix + "ENDPOINT", false);
        public static string Key => McmaEnvironmentVariables.Get(EnvironmentVariablePrefix + "KEY", false);
        public static string DatabaseId => McmaEnvironmentVariables.Get(EnvironmentVariablePrefix + "DATABASE_ID", false);
        public static string Region => McmaEnvironmentVariables.Get(EnvironmentVariablePrefix + "REGION", false);
    }
}