using System;

namespace Mcma.Data.Aws.DynamoDb.AttributeMapping
{
    public interface IAttributeMapping
    {
        string Name { get; }
        
        Type ResourceType { get; }
        
        object Get(string partitionKey, string sortKey, object input);
    }
}