using Mcma.Data.DocumentDatabase.Queries;
using FirestoreQuery = Google.Cloud.Firestore.Query;

namespace Mcma.Data.Google.Firestore;

public interface IFirestoreQueryBuilder
{
    FirestoreQuery Build<T>(FirestoreQuery firestoreQuery, Query<T> query);
}