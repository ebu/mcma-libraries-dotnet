using System.Collections.Generic;

namespace Mcma.Aws.DynamoDb
{
    public interface IAttributeMapper
    {
        Dictionary<string, object> GetMappedAttributes<TResource>(string partitionKey, string sortKey, TResource resource);
    }
}