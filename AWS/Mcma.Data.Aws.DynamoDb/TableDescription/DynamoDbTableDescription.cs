using System;

namespace Mcma.Data.Aws.DynamoDb.TableDescription;

public class DynamoDbTableDescription
{
    public DynamoDbTableDescription(string tableName,
                                    string partitionKeyName,
                                    string sortKeyName,
                                    LocalSecondaryIndexDescription[] localSecondaryIndexes,
                                    GlobalSecondaryIndexDescription[] globalSecondaryIndexes)
    {
        TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        KeyNames = new KeyNames(partitionKeyName, sortKeyName);
        LocalSecondaryIndexes = localSecondaryIndexes ?? [];
        GlobalSecondaryIndexes = globalSecondaryIndexes ?? [];
    }

    public string TableName { get; }

    public KeyNames KeyNames { get; }

    public LocalSecondaryIndexDescription[] LocalSecondaryIndexes { get; }

    public GlobalSecondaryIndexDescription[] GlobalSecondaryIndexes { get; }
}