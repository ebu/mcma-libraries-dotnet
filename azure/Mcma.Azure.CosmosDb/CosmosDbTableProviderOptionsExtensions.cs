using System;
using System.Linq;
using Mcma;
using Mcma.Context;
using Mcma.Utility;

namespace Mcma.Azure.CosmosDb
{
    public static class CosmosDbTableProviderOptionsExtensions
    {
        private const string Prefix = "CosmosDb";

        public static CosmosDbTableProviderConfiguration FromEnvironmentVariables(this CosmosDbTableProviderConfiguration configuration)
        {
            foreach (var prop in typeof(CosmosDbTableProviderConfiguration).GetProperties().Where(p => p.CanWrite))
                prop.SetValue(configuration, Environment.GetEnvironmentVariable(Prefix + prop.Name));

            return configuration;
        }

        public static CosmosDbTableProviderConfiguration FromContextVariables(this CosmosDbTableProviderConfiguration configuration, IContextVariableProvider contextVariableProvider)
        {
            foreach (var prop in typeof(CosmosDbTableProviderConfiguration).GetProperties().Where(p => p.CanWrite))
            {
                var contextVariableValue = contextVariableProvider.GetRequiredContextVariable(Prefix + prop.Name);
                if (!contextVariableValue.TryParse(prop.PropertyType, out var propValue))
                    throw new Exception($"Context variable '{Prefix + prop.Name}' has invalid value '{contextVariableValue}' for Cosmos DB option '{prop.Name}'.");

                prop.SetValue(configuration, propValue);
            }

            return configuration;
        }
    }
}
