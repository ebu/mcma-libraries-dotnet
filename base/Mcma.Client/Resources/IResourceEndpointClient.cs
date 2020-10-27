using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mcma.Client
{
    public interface IResourceEndpointClient
    {
        string HttpEndpoint { get; }
        
        Task<QueryResults<T>> QueryAsync<T>(string url = null,
                                            IDictionary<string, string> queryParameters = null,
                                            CancellationToken cancellationToken = default)
            where T : McmaObject;

        Task<T> PostAsync<T>(T body, string url = null, CancellationToken cancellationToken = default) where T : McmaObject;
        
        Task<T> GetAsync<T>(string url = null, CancellationToken cancellationToken = default) where T : McmaObject;

        Task<T> PutAsync<T>(T body, string url = null, CancellationToken cancellationToken = default) where T : McmaObject;

        Task DeleteAsync<T>(string url = null, CancellationToken cancellationToken = default) where T : McmaObject;
    }
}