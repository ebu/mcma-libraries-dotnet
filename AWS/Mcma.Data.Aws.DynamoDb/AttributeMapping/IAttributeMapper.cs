using System.Collections.Generic;

namespace Mcma.Data.Aws.DynamoDb.AttributeMapping
{
    public interface IAttributeMapper
    {
        Dictionary<string, object> GetMappedAttributes<TResource>(string partitionKey, string sortKey, TResource resource);
    }
}