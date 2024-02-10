#if NET48_OR_GREATER
using System.Net.Http;
#endif
using Mcma.Client.Auth;
using Mcma.Client.Http;
using Mcma.Model;

namespace Mcma.Client.Resources;

internal class ResourceEndpointClient : IResourceEndpointClient
{
    internal ResourceEndpointClient(IAuthProvider authProvider,
                                    HttpClient httpClient,
                                    Service service,
                                    ResourceEndpoint resourceEndpoint,
                                    McmaTracker? tracker)
    {
        AuthProvider = authProvider ?? throw new ArgumentNullException(nameof(authProvider));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Service = service ?? throw new ArgumentNullException(nameof(service));
        ResourceEndpoint = resourceEndpoint ?? throw new ArgumentNullException(nameof(resourceEndpoint));

        AuthType = !string.IsNullOrWhiteSpace(resourceEndpoint.AuthType) ? resourceEndpoint.AuthType : service.AuthType;
        Tracker = tracker;
        McmaHttpClient = new(GetMcmaHttpClient);
    }

    private HttpClient HttpClient { get; }

    private McmaTracker? Tracker { get; }

    private IAuthProvider AuthProvider { get; }
    
    private Service Service { get; }

    private ResourceEndpoint ResourceEndpoint { get; }

    private string? AuthType { get; }
        
    private Lazy<McmaHttpClient> McmaHttpClient { get; set; }

    public string HttpEndpoint => ResourceEndpoint.HttpEndpoint;

    private McmaHttpClient GetMcmaHttpClient()
    {
        var authenticator =
            AuthProvider != null && !string.IsNullOrWhiteSpace(AuthType)
                ? AuthProvider.Get(AuthType!, Service.Name, ResourceEndpoint.ResourceType)
                : null;

        return new McmaHttpClient(HttpClient, authenticator, Tracker);
    }

    private string GetFullUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return HttpEndpoint;

        if (url!.StartsWith(HttpEndpoint, StringComparison.OrdinalIgnoreCase))
            return url;
            
        return HttpEndpoint?.TrimEnd('/') + "/" + url;
    }

    public Task<QueryResults<T>> QueryAsync<T>(string? url = null,
                                               params (string Key, string Value)[] queryParameters)
        where T : McmaObject
        => QueryAsync<T>(url, CancellationToken.None, queryParameters);

    public Task<QueryResults<T>> QueryAsync<T>(CancellationToken cancellationToken,
                                               params (string Key, string Value)[] queryParameters)
        where T : McmaObject
        => QueryAsync<T>(null, cancellationToken, queryParameters);

    public async Task<QueryResults<T>> QueryAsync<T>(string? url,
                                                     CancellationToken cancellationToken,
                                                     params (string Key, string Value)[] queryParameters)
        where T : McmaObject
    {
        var mcmaHttpClient = GetMcmaHttpClient();
            
        url = GetFullUrl(url);
        if (queryParameters != null && queryParameters.Any())
            url += $"?{string.Join("&", queryParameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
            
        return await mcmaHttpClient.GetAsync<QueryResults<T>>(url, cancellationToken: cancellationToken);
    }

    private async Task ExecuteAsync(Func<McmaHttpClient, Task> executeAsync) => await executeAsync(McmaHttpClient.Value);

    private async Task<T> ExecuteAsync<T>(Func<McmaHttpClient, Task<T>> executeAsync) => await executeAsync(McmaHttpClient.Value);

    public Task PostAsync(object body, string? relativePath = null, CancellationToken cancellationToken = default)
        => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.PostAsync(HttpEndpoint, body, cancellationToken));

    public Task<T> PostAsync<T>(object body, string? relativePath = null, CancellationToken cancellationToken = default) where T : McmaObject
        => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.PostAsync<T>(HttpEndpoint, body, cancellationToken));

    public Task<T?> GetAsync<T>(string relativePath, CancellationToken cancellationToken = default) where T : McmaObject
        => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.GetWithNullOn404Async<T>(GetFullUrl(relativePath), cancellationToken));

    public Task<T[]> GetCollectionAsync<T>(string relativePath, CancellationToken cancellationToken = default) where T : McmaObject
        => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.GetAsync<T[]>($"{GetFullUrl(relativePath)}", cancellationToken));

    public Task PutAsync(string relativePath, object body, CancellationToken cancellationToken = default)
        => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.PutAsync(GetFullUrl(relativePath), body, cancellationToken));

    public Task<T> PutAsync<T>(string relativePath, object body, CancellationToken cancellationToken = default) where T : McmaObject
        => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.PutAsync<T>(GetFullUrl(relativePath), body, cancellationToken));

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
        => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.DeleteAsync(GetFullUrl(relativePath), cancellationToken));
}