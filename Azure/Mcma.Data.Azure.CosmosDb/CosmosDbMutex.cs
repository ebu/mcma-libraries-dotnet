using System;
using System.Net;
using System.Threading.Tasks;
using Mcma.Data.DocumentDatabase;
using Mcma.Logging;
using Mcma.Serialization;
using Microsoft.Azure.Cosmos;

namespace Mcma.Data.Azure.CosmosDb;

public class CosmosDbMutex : DocumentDatabaseMutex
{
    public CosmosDbMutex(Container container, string partitionKeyName, string mutexName, string mutexHolder, TimeSpan? lockTimeout, ILogger logger = null)
        : base(mutexName, mutexHolder, lockTimeout, logger)
    {
        Container = container;
        PartitionKeyName = partitionKeyName;
    }
        
    private Container Container { get; }

    private string PartitionKeyName { get; }

    private string ETag { get; set; }

    protected override string VersionId => ETag;

    protected override async Task PutLockDataAsync()
    {
        var item = new LockData
        {
            MutexHolder = MutexHolder,
            Timestamp = DateTimeOffset.UtcNow
        }.ToMcmaJson();

        item["id"] = Uri.EscapeDataString($"Mutex-{MutexName}");

        PartitionKey partitionKey;
        if (!string.IsNullOrWhiteSpace(PartitionKeyName))
        {
            item[PartitionKeyName] = "Mutex";
            partitionKey = new PartitionKey("Mutex");
        }
        else
            partitionKey = PartitionKey.None;
            
        var response = await Container.CreateItemAsync(item, partitionKey);
        
        ETag = response.ETag;
    }

    protected override async Task<LockData> GetLockDataAsync()
    {
        var id = Uri.EscapeDataString($"Mutex-{MutexName}");
        var partitionKey = !string.IsNullOrWhiteSpace(PartitionKeyName) ? new PartitionKey("Mutex") : PartitionKey.None;

        var resp =
            await Container.ReadItemStreamAsync(id, partitionKey, new ItemRequestOptions {ConsistencyLevel = ConsistencyLevel.Strong});
        if (resp.StatusCode == HttpStatusCode.NotFound)
            return null;

        resp.EnsureSuccessStatusCode();

        var resource = await resp.ToObjectAsync<LockData>();
        if (resource == null)
            return null;

        // sanity check which removes the record from CosmosDB in case it has incompatible structure. Only possible
        // if modified externally, but this could lead to a situation where the lock would never be acquired.
        if (resource.MutexHolder == null || resource.Timestamp == default)
        {
            await Container.DeleteItemAsync<LockData>(id, partitionKey);
            return null;
        }

        resource.VersionId = resp.Headers.ETag;
        
        return resource;
    }

    protected override async Task DeleteLockDataAsync(string versionId)
    {
        var id = Uri.EscapeDataString($"Mutex-{MutexName}");
        var partitionKey = !string.IsNullOrWhiteSpace(PartitionKeyName) ? new PartitionKey("Mutex") : PartitionKey.None;

        await Container.DeleteItemAsync<LockData>(id, partitionKey, new ItemRequestOptions {IfMatchEtag = versionId});
    }
}