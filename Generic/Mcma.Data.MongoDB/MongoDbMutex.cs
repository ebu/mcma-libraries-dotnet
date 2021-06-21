using System;
using System.Linq;
using System.Threading.Tasks;
using Mcma.Data.DocumentDatabase;
using Mcma.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mcma.Data.MongoDB
{
    public class MongoDbMutex : DocumentDatabaseMutex
    {
        public MongoDbMutex(IMongoCollection<BsonDocument> collection, string mutexName, string mutexHolder, TimeSpan? lockTimeout, ILogger logger = null)
            : base(mutexName, mutexHolder, lockTimeout, logger)
        {
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }
        
        private IMongoCollection<BsonDocument> Collection { get; }

        protected override string VersionId { get; } = Guid.NewGuid().ToString();

        private string Key => $"Mutex-{MutexName}";

        protected override async Task<LockData> GetLockDataAsync()
        {
            var filter = Builders<BsonDocument>.Filter.Where(x => x["_id"] == Key);
 
            var cursor = await Collection.FindAsync(filter);
            if (!await cursor.MoveNextAsync())
                return null;

            var record = cursor.Current.FirstOrDefault();
            if (record == null)
                return null;

            if (!record.TryGetValue(nameof(LockData.MutexHolder), out var mutexHolder) ||
                !record.TryGetValue(nameof(LockData.VersionId), out var versionId) ||
                !record.TryGetValue(nameof(LockData.Timestamp), out var timestamp))
            {
                await Collection.DeleteOneAsync(filter);
                return null;
            }

            return new LockData
            {
                MutexHolder = mutexHolder.AsString,
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestamp.AsInt64),
                VersionId = versionId.AsString
            };
        }

        protected override async Task PutLockDataAsync()
        {
            var doc =
                new BsonDocument
                {
                    ["_id"] = Key,
                    [nameof(MutexName)] = MutexName,
                    [nameof(LockData.MutexHolder)] = MutexHolder,
                    [nameof(LockData.VersionId)] = VersionId,
                    [nameof(LockData.Timestamp)] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };

            var filter = Builders<BsonDocument>.Filter.Where(x => x["_id"] == Key);

            await Collection.ReplaceOneAsync(filter, doc, new ReplaceOptions {IsUpsert = true});
        }

        protected override async Task DeleteLockDataAsync(string versionId)
        {
            var filter = Builders<BsonDocument>.Filter.Where(x => x["_id"] == Key && x["VersionId"] == versionId);
            
            await Collection.DeleteOneAsync(filter);
        }
    }
}