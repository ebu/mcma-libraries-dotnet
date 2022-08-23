using System;

namespace Mcma.Data.Aws.DynamoDb.AttributeMapping;

public class AttributeMapping<TResource> : IAttributeMapping
{
    public AttributeMapping(string name, Func<string, string, TResource, object> get)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Get = get ?? throw new ArgumentNullException(nameof(get));
    }
        
    public string Name { get; }

    private Func<string, string, TResource, object> Get { get; }

    public Type ResourceType => typeof(TResource);

    object IAttributeMapping.Get(string partitionKey, string sortKey, object input) => Get(partitionKey, sortKey, (TResource)input);
}