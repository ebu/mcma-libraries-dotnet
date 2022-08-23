using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Mcma.Data.Aws.DynamoDb.TableDescription;
using Mcma.Data.DocumentDatabase;
using Mcma.Logging;

namespace Mcma.Data.Aws.DynamoDb;

public class DynamoDbMutex : DocumentDatabaseMutex
{
    public DynamoDbMutex(Table table, DynamoDbTableDescription tableDescription, string mutexName, string mutexHolder, TimeSpan? lockTimeout = null, ILogger logger = null)
        : base(mutexName, mutexHolder, lockTimeout, logger)
    {
        Table = table;
        TableDescription = tableDescription;
    }
        
    private Table Table { get; }
        
    private DynamoDbTableDescription TableDescription { get; }

    protected override string VersionId { get; } = Guid.NewGuid().ToString();

    private Document GenerateTableKey()
    {
        var key = new Document();
        if (TableDescription.KeyNames.Sort != null)
        {
            key[TableDescription.KeyNames.Partition] = "Mutex";
            key[TableDescription.KeyNames.Sort] = MutexName;
        }
        else
            key[TableDescription.KeyNames.Partition] = $"Mutex-{MutexName}";
        return key;
    }

    private Document GenerateTableItem()
    {
        var item = GenerateTableKey();
        item[nameof(LockData.MutexHolder)] = MutexHolder;
        item[nameof(LockData.VersionId)] = VersionId;
        item[nameof(LockData.Timestamp)] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return item;
    }

    protected override async Task<LockData> GetLockDataAsync()
    {
        var key = GenerateTableKey();
 
        var record = await Table.GetItemAsync(key, new GetItemOperationConfig {ConsistentRead = true});
        if (record == null)
            return null;

        if (!record.ContainsKey(nameof(LockData.MutexHolder)) ||
            !record.ContainsKey(nameof(LockData.VersionId)) ||
            !record.ContainsKey(nameof(LockData.Timestamp)))
        {
            await Table.DeleteItemAsync(key);

            return null;
        }

        return new LockData
        {
            MutexHolder = record[nameof(LockData.MutexHolder)],
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(record[nameof(LockData.Timestamp)])),
            VersionId = record[nameof(LockData.VersionId)]
        };
    }

    protected override async Task PutLockDataAsync()
    {
        await Table.PutItemAsync(GenerateTableItem(),
                                 new PutItemOperationConfig
                                 {
                                     ConditionalExpression =
                                         new Expression
                                         {
                                             ExpressionStatement = "attribute_not_exists(#res_id) and attribute_not_exists(#p_key)",
                                             ExpressionAttributeNames = new Dictionary<string, string>
                                             {
                                                 ["#res_id"] = "resource_id",
                                                 ["#p_key"] = TableDescription.KeyNames.Partition
                                             }
                                         }
                                 });
    }

    protected override async Task DeleteLockDataAsync(string versionId)
    {
        await Table.DeleteItemAsync(GenerateTableKey(),
                                    new DeleteItemOperationConfig
                                    {
                                        ConditionalExpression =
                                            new Expression
                                            {
                                                ExpressionStatement = "#v_id = :v_id",
                                                ExpressionAttributeNames =
                                                    new Dictionary<string, string> {["#v_id"] = nameof(LockData.VersionId)},
                                                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry> {[":v_id"] = VersionId}
                                            }
                                    });
    }
}