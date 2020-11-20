﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Mcma.Data;
using Mcma.Data.DocumentDatabase.Queries;
using Microsoft.Extensions.Options;
using FirestoreQuery = Google.Cloud.Firestore.Query;

namespace Mcma.GoogleCloud.Firestore
{
    public class FirestoreTable : IDocumentDatabaseTable
    {
        public FirestoreTable(ICustomQueryBuilderRegistry<(FirestoreQuery, string)> customQueryBuilderRegistry,
                              IFirestoreQueryBuilder queryBuilder,
                              FirestoreDb firestore,
                              IOptions<FirestoreTableOptions> options)
        {
            CustomQueryBuilderRegistry = customQueryBuilderRegistry ?? throw new ArgumentNullException(nameof(customQueryBuilderRegistry));
            QueryBuilder = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));
            
            var tableOptions = options.Value ?? new DocumentDatabaseTableOptions();
            RootCollection = firestore.Collection(tableOptions.TableName);
        }

        private ICustomQueryBuilderRegistry<(FirestoreQuery, string)> CustomQueryBuilderRegistry { get; }

        private IFirestoreQueryBuilder QueryBuilder { get; }

        private CollectionReference RootCollection { get; }

        private CollectionReference Collection(string path) => RootCollection.Document("resources").Collection(path);

        private DocumentReference Document(string id) => RootCollection.Document("resources" + id);
        
        public async Task<QueryResults<T>> QueryAsync<T>(Query<T> query) where T : class
        {
            var firestoreQuery = QueryBuilder.Build(Collection(query.Path), query);

            var results = await firestoreQuery.GetSnapshotAsync();

            return new QueryResults<T>
            {
                Results = results.Documents.Select(d => d.ConvertTo<T>()).ToArray(),
                NextPageStartToken = await results.GetNextPageStartTokenAsync(query.SortBy)
            };
        }

        public async Task<QueryResults<TResource>> CustomQueryAsync<TResource, TParameters>(CustomQuery<TParameters> customQuery) where TResource : class
        {
            var customQueryBuilder = CustomQueryBuilderRegistry.Get<TParameters>(customQuery.Name);

            var (firestoreQuery, sortBy) = customQueryBuilder.Build(customQuery);
            
            if (customQuery.PageStartToken != null)
                firestoreQuery = firestoreQuery.StartAt(customQuery.PageStartToken);

            var results = await firestoreQuery.GetSnapshotAsync();

            return new QueryResults<TResource>
            {
                Results = results.Documents.Select(d => d.ConvertTo<TResource>()).ToArray(),
                NextPageStartToken = await results.GetNextPageStartTokenAsync(sortBy)
            };
        }

        public async Task<T> GetAsync<T>(string id) where T : class
        {
            var doc = await Document(id).GetSnapshotAsync();
            return doc.ConvertTo<T>();
        }

        public async Task<T> PutAsync<T>(string id, T resource) where T : class
        {
            await Document(id).SetAsync(resource);
            return resource;
        }

        public async Task DeleteAsync(string id)
        {
            await Document(id).DeleteAsync();
        }

        public Task<IDocumentDatabaseMutex> CreateMutexAsync(string mutexName, string mutexHolder, TimeSpan? lockTimeout = null)
        {
            return Task.FromResult<IDocumentDatabaseMutex>(new FirestoreMutex(RootCollection, mutexName, mutexHolder, lockTimeout));
        }
    }
}