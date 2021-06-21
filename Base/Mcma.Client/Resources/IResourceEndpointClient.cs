using System.Threading;
using System.Threading.Tasks;
using Mcma.Model;

namespace Mcma.Client.Resources
{
    public interface IResourceEndpointClient
    {
        string HttpEndpoint { get; }
        
        Task<QueryResults<T>> QueryAsync<T>(string url = null,
                                            params (string Key, string Value)[] queryParameters)
            where T : McmaObject;
        
        Task<QueryResults<T>> QueryAsync<T>(CancellationToken cancellationToken,
                                            params (string Key, string Value)[] queryParameters)
            where T : McmaObject;
        
        Task<QueryResults<T>> QueryAsync<T>(string url,
                                            CancellationToken cancellationToken,
                                            params (string Key, string Value)[] queryParameters)
            where T : McmaObject;

        Task<T> PostAsync<T>(object body, string url = null, CancellationToken cancellationToken = default) where T : McmaObject;

        Task PostAsync(object body, string url = null, CancellationToken cancellationToken = default);
        
        Task<T> GetAsync<T>(string url = null, CancellationToken cancellationToken = default) where T : McmaObject;

        Task<T> PutAsync<T>(object body, string url = null, CancellationToken cancellationToken = default) where T : McmaObject;

        Task PutAsync(object body, string url = null, CancellationToken cancellationToken = default);

        Task DeleteAsync(string url = null, CancellationToken cancellationToken = default);
    }
}