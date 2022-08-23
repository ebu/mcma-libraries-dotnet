using System;

namespace Mcma.Data.Aws.DynamoDb.TableDescription;

public readonly struct GlobalSecondaryIndexDescription
{
    public GlobalSecondaryIndexDescription(string name, string partitionKeyName, string sortKeyName)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        KeyNames =
            new KeyNames(
                partitionKeyName ?? throw new ArgumentNullException(nameof(partitionKeyName)),
                sortKeyName);
    }
    public string Name { get; }
        
    public KeyNames KeyNames { get; }
}