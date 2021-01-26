using System;
using System.Collections.Generic;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Azure.Cosmos;

namespace Mcma.Azure.CosmosDb
{
    public class CosmosDbTableOptions
    {
        public delegate (QueryDefinition, QueryRequestOptions) BuildCustomQuery<TParameters>(CustomQuery<TParameters> customQuery);

        private Dictionary<Type, Dictionary<string, object>> CustomQueryRegistry { get; } =
            new Dictionary<Type, Dictionary<string, object>>();
        
        public bool? ConsistentGet { get; set; }
        
        public bool? ConsistentQuery { get; set; }

        public CosmosDbTableOptions AddCustomQueryBuilder<TParameters>(string name, BuildCustomQuery<TParameters> createCustomQuery)
        {
            if (!CustomQueryRegistry.ContainsKey(typeof(TParameters)))
                CustomQueryRegistry[typeof(TParameters)] = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            
            CustomQueryRegistry[typeof(TParameters)][name] = createCustomQuery;

            return this;
        }

        public BuildCustomQuery<TParameters> GetCustomQueryBuilder<TParameters>(string name)
            =>
                CustomQueryRegistry.ContainsKey(typeof(TParameters)) && CustomQueryRegistry[typeof(TParameters)].ContainsKey(name)
                    ? (BuildCustomQuery<TParameters>)CustomQueryRegistry[typeof(TParameters)][name]
                    : throw new McmaException(
                          $"No custom query configured with name '{name}' that accepts parameters of type {typeof(TParameters).Name}");
    }
}