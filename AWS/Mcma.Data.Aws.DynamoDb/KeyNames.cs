using System;

namespace Mcma.Data.Aws.DynamoDb;

public readonly struct KeyNames
{
    public KeyNames(string partition, string sort)
    {
        Partition = partition ?? throw new ArgumentNullException(nameof(partition));
        Sort = sort;
    }

    public string Partition { get; }

    public string Sort { get; }
}