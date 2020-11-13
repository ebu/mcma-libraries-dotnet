using Mcma.Data.DocumentDatabase.Queries;
using FirestoreQuery = Google.Cloud.Firestore.Query;

namespace Mcma.GoogleCloud.Firestore
{
    public interface IFirestoreQueryBuilder
    {
        FirestoreQuery Build<T>(FirestoreQuery firestoreQuery, Query<T> query);
    }
}