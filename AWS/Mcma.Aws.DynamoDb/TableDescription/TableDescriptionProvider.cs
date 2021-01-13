using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;

namespace Mcma.Aws.DynamoDb
{
    public class TableDescriptionProvider : ITableDescriptionProvider
    {
        private Dictionary<string, DynamoDbTableDescription> TableDescriptions { get; } =
            new Dictionary<string, DynamoDbTableDescription>(StringComparer.OrdinalIgnoreCase);

        private SemaphoreSlim TableDescriptionsSemaphore { get; } = new SemaphoreSlim(1, 1);

        public async Task<DynamoDbTableDescription> GetTableDescriptionAsync(IAmazonDynamoDB dynamoDb, string tableName)
        {
            await TableDescriptionsSemaphore.WaitAsync();

            try
            {
                if (TableDescriptions.ContainsKey(tableName))
                    return TableDescriptions[tableName];
                
                var data = await dynamoDb.DescribeTableAsync(tableName);

                string partitionKeyName = null;
                string sortKeyName = null;
                foreach (var key in data.Table.KeySchema)
                {
                    if (key.KeyType == KeyType.HASH)
                        partitionKeyName = key.AttributeName;
                    else if (key.KeyType == KeyType.RANGE)
                        sortKeyName = key.AttributeName;
                }

                var localSecondaryIndexes =
                    data.Table.LocalSecondaryIndexes
                        .Select(
                            lsi =>
                                new LocalSecondaryIndexDescription(
                                    lsi.IndexName,
                                    lsi.KeySchema.FirstOrDefault(k => k.KeyType == KeyType.RANGE)?.AttributeName))
                        .ToArray();

                var globalSecondaryIndexes =
                    data.Table.GlobalSecondaryIndexes
                        .Select(
                            gsi =>
                                new GlobalSecondaryIndexDescription(
                                    gsi.IndexName,
                                    gsi.KeySchema.FirstOrDefault(k => k.KeyType == KeyType.HASH)?.AttributeName,
                                    gsi.KeySchema.FirstOrDefault(k => k.KeyType == KeyType.RANGE)?.AttributeName))
                        .ToArray();

                TableDescriptions[tableName] =
                    new DynamoDbTableDescription(tableName, partitionKeyName, sortKeyName, localSecondaryIndexes, globalSecondaryIndexes);

                return TableDescriptions[tableName];
            }
            finally
            {
                TableDescriptionsSemaphore.Release();
            }
        }
    }
}