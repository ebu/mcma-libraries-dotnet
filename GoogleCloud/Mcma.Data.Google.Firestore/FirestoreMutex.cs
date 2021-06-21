using System;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Mcma.Data;
using Mcma.Data.DocumentDatabase;

namespace Mcma.Data.Google.Firestore
{
    public class FirestoreMutex : DocumentDatabaseMutex
    {
        public FirestoreMutex(CollectionReference collection, string mutexName, string mutexHolder, TimeSpan? lockTimeout = null)
            : base(mutexName, mutexHolder, lockTimeout)
        {
            Document = collection.Document($"mutexes/{MutexName.TrimStart('/')}");
        }
        
        private DocumentReference Document { get; }
        
        private string Timestamp { get; set; }

        protected override string VersionId => Timestamp;

        protected override async Task<LockData> GetLockDataAsync()
        {
            var docSnapshot = await Document.GetSnapshotAsync();
            var item = docSnapshot.ConvertTo<LockData>();
            
            if (item == null)
                return null;

            if (item.MutexHolder == null || item.Timestamp == default)
            {
                await Document.DeleteAsync();
                return null;
            }

            item.VersionId = docSnapshot.UpdateTime?.ToString();

            return item;
        }

        protected override async Task PutLockDataAsync()
        {
            var result = await Document.CreateAsync(new
            {
                MutexHolder,
                Timestamp = DateTimeOffset.UtcNow
            });

            Timestamp = result.UpdateTime.ToDateTimeOffset().ToString();
        }

        protected override async Task DeleteLockDataAsync(string versionId)
        {
            var lastUpdateTime = global::Google.Cloud.Firestore.Timestamp.FromDateTimeOffset(DateTimeOffset.Parse(versionId));
            await Document.DeleteAsync(Precondition.LastUpdated(lastUpdateTime));
        }
    }
}