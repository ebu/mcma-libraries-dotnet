using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data.DocumentDatabase.Queries;

namespace Mcma.Aws.DynamoDb
{
    public class DynamoDbTableProviderOptions
    {
        public delegate QueryOperationConfig BuildCustomQuery<TParameters>(CustomQuery<TParameters> customQuery);
        
        private Dictionary<Type, Dictionary<string, Func<string, string, object, object>>> TopLevelAttributeMappings { get; } =
            new Dictionary<Type, Dictionary<string, Func<string, string, object, object>>>();

        private Dictionary<Type, Dictionary<string, object>> CustomQueryRegistry { get; } =
            new Dictionary<Type, Dictionary<string, object>>();
        
        public bool? ConsistentGet { get; set; }
        
        public bool? ConsistentQuery { get; set; }

        public DynamoDbTableProviderOptions AddTopLevelAttribute<TResource>(string name, Func<string, string, TResource, object> retrieveValue)
            where TResource : McmaObject
        {
            if (!TopLevelAttributeMappings.ContainsKey(typeof(TResource)))
                TopLevelAttributeMappings[typeof(TResource)] =
                    new Dictionary<string, Func<string, string, object, object>>(StringComparer.OrdinalIgnoreCase);
            
            TopLevelAttributeMappings[typeof(TResource)][name] =
                (partitionKey, sortKey, resource) => retrieveValue(partitionKey, sortKey, (TResource)resource);
            
            return this;
        }

        public Dictionary<string, object> GetTopLevelAttributes<TResource>(string partitionKey, string sortKey, TResource resource)
        {
            if (!TopLevelAttributeMappings.ContainsKey(typeof(TResource)))
                return new Dictionary<string, object>();

            return TopLevelAttributeMappings[typeof(TResource)]
                .ToDictionary(
                    kvp => kvp.Key, 
                    kvp => kvp.Value(partitionKey, sortKey, resource));
        }

        public DynamoDbTableProviderOptions AddCustomQueryBuilder<TParameters>(string name, BuildCustomQuery<TParameters> createCustomQuery)
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