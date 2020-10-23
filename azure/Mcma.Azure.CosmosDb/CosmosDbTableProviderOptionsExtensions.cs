using System.Linq;

namespace Mcma.Azure.CosmosDb
{
    public static class CosmosDbTableProviderOptionsExtensions
    {
        private const string Prefix = "CosmosDb";

        public static CosmosDbTableProviderConfiguration FromEnvironmentVariables(this CosmosDbTableProviderConfiguration configuration, IEnvironmentVariables environmentVariables = null)
        {
            environmentVariables ??= EnvironmentVariables.Instance;
            
            foreach (var prop in typeof(CosmosDbTableProviderConfiguration).GetProperties().Where(p => p.CanWrite))
                prop.SetValue(configuration, environmentVariables.Get(Prefix + prop.Name));

            return configuration;
        }
    }
}
