using Mcma.Model;

namespace Mcma.Client.Resources;

public interface IResourceEndpointClient
{
    string HttpEndpoint { get; }
        
    Task<QueryResults<T>> QueryAsync<T>(string? url = null,
                                        params (string Key, string Value)[] queryParameters)
        where T : McmaObject;
        
    Task<QueryResults<T>> QueryAsync<T>(CancellationToken cancellationToken,
                                        params (string Key, string Value)[] queryParameters)
        where T : McmaObject;
        
    Task<QueryResults<T>> QueryAsync<T>(string url,
                                        CancellationToken cancellationToken,
                                        params (string Key, string Value)[] queryParameters)
        where T : McmaObject;

    Task<T> PostAsync<T>(object body, string? relativePath = null, CancellationToken cancellationToken = default) where T : McmaObject;

    Task PostAsync(object body, string? relativePath = null, CancellationToken cancellationToken = default);
        
    Task<T?> GetAsync<T>(string relativePath, CancellationToken cancellationToken = default) where T : McmaObject;

    Task<T[]> GetCollectionAsync<T>(string relativePath, CancellationToken cancellationToken = default) where T : McmaObject;

    Task<T> PutAsync<T>(string relativePath, object body, CancellationToken cancellationToken = default) where T : McmaObject;

    Task PutAsync(string relativePath, object body, CancellationToken cancellationToken = default);

    Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default);
}