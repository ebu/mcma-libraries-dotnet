#if NET48_OR_GREATER
using System.Net.Http;
#endif
using Mcma.Client.Auth;
using Mcma.Model;
using Mcma.Serialization;
using Mcma.Utility;

namespace Mcma.Client.Http;

public class McmaHttpClient
{
    public McmaHttpClient(HttpClient httpClient, IAuthenticator authenticator = null, McmaTracker tracker = null)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Authenticator = authenticator;
        Tracker = tracker;
    }

    private HttpClient HttpClient { get; }

    private IAuthenticator Authenticator { get; }

    private McmaTracker Tracker { get; }

    public Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default)
        => SendAsync(new HttpRequestMessage(HttpMethod.Get, url), cancellationToken);

    public Task<HttpResponseMessage> PostAsync(string url, HttpContent body, CancellationToken cancellationToken = default)
        => SendAsync(new HttpRequestMessage(HttpMethod.Post, url) {Content = body}, cancellationToken);

    public Task<HttpResponseMessage> PutAsync(string url, HttpContent body,  CancellationToken cancellationToken = default)
        => SendAsync(new HttpRequestMessage(HttpMethod.Put, url) {Content = body}, cancellationToken);

    public Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken cancellationToken = default)
        => SendAsync(new HttpRequestMessage(HttpMethod.Delete, url), cancellationToken);

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default)
    {
        if (Tracker != null)
        {
            if (httpRequestMessage.Headers.Any(h => h.Key == McmaHttpHeaders.Tracker))
                httpRequestMessage.Headers.Remove(McmaHttpHeaders.Tracker);
                
            httpRequestMessage.Headers.Add(McmaHttpHeaders.Tracker, Tracker.ToMcmaJson().ToString().ToBase64());
        }
            
        if (Authenticator != null)
            await Authenticator.AuthenticateAsync(httpRequestMessage, cancellationToken);

        return await HttpClient.SendAsync(httpRequestMessage, cancellationToken);
    }
}