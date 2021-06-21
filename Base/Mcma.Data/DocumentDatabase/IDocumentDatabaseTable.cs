using System;
using System.Threading.Tasks;
using Mcma.Data.DocumentDatabase.Queries;
using Mcma.Data.DocumentDatabase.Queries.Custom;
using Mcma.Model;

namespace Mcma.Data.DocumentDatabase
{
    public interface IDocumentDatabaseTable
    {
        Task<QueryResults<T>> QueryAsync<T>(Query<T> query) where T : class;

        Task<QueryResults<TResource>> CustomQueryAsync<TResource, TParameters>(CustomQuery<TParameters> customQuery) where TResource : class;

        Task<T> GetAsync<T>(string id) where T : class;

        Task<T> PutAsync<T>(string id, T resource) where T : class;

        Task DeleteAsync(string id);

        Task<IDocumentDatabaseMutex> CreateMutexAsync(string mutexName, string mutexHolder, TimeSpan? lockTimeout = null);
    }
}
