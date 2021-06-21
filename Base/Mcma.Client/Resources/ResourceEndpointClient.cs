using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.Auth;
using Mcma.Client.Http;
using Mcma.Model;

namespace Mcma.Client.Resources
{
    internal class ResourceEndpointClient : IResourceEndpointClient
    {
        internal ResourceEndpointClient(IAuthProvider authProvider,
                                        HttpClient httpClient,
                                        ResourceEndpoint resourceEndpoint,
                                        string serviceAuthType,
                                        object serviceAuthContext,
                                        McmaTracker tracker)
        {
            AuthProvider = authProvider ?? throw new ArgumentNullException(nameof(authProvider));
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            HttpEndpoint = resourceEndpoint?.HttpEndpoint ?? throw new ArgumentNullException(nameof(resourceEndpoint));

            AuthType = !string.IsNullOrWhiteSpace(resourceEndpoint.AuthType) ? resourceEndpoint.AuthType : serviceAuthType;
            AuthContext = resourceEndpoint.AuthContext ?? serviceAuthContext;
            Tracker = tracker;
        }

        private HttpClient HttpClient { get; }

        private McmaTracker Tracker { get; }

        private IAuthProvider AuthProvider { get; }

        private string AuthType { get; }

        private object AuthContext { get; }

        public string HttpEndpoint { get; }
        
        private McmaHttpClient McmaHttpClient { get; set; }

        private async Task<McmaHttpClient> GetMcmaHttpClient()
        {
            if (McmaHttpClient != null)
                return McmaHttpClient;

            var authenticator = AuthProvider != null && !string.IsNullOrWhiteSpace(AuthType)
                                    ? await AuthProvider.GetAsync(AuthType, AuthContext)
                                    : null;

            McmaHttpClient = new McmaHttpClient(HttpClient, authenticator, Tracker);

            return McmaHttpClient;
        }

        private string GetFullUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return HttpEndpoint;

            if (url.StartsWith(HttpEndpoint, StringComparison.OrdinalIgnoreCase))
                return url;
            
            return HttpEndpoint?.TrimEnd('/') + "/" + url;
        }

        public Task<QueryResults<T>> QueryAsync<T>(string url = null,
                                                   params (string Key, string Value)[] queryParameters)
            where T : McmaObject
            => QueryAsync<T>(url, CancellationToken.None, queryParameters);

        public Task<QueryResults<T>> QueryAsync<T>(CancellationToken cancellationToken,
                                                   params (string Key, string Value)[] queryParameters)
            where T : McmaObject
            => QueryAsync<T>(null, cancellationToken, queryParameters);

        public async Task<QueryResults<T>> QueryAsync<T>(string url,
                                                         CancellationToken cancellationToken,
                                                         params (string Key, string Value)[] queryParameters)
            where T : McmaObject
        {
            var mcmaHttpClient = await GetMcmaHttpClient();
            
            url = GetFullUrl(url);
            if (queryParameters != null && queryParameters.Any())
                url += $"?{string.Join("&", queryParameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
            
            return await mcmaHttpClient.GetAsync<QueryResults<T>>(url, cancellationToken: cancellationToken);
        }

        private async Task ExecuteAsync(Func<McmaHttpClient, Task> executeAsync) => await executeAsync(await GetMcmaHttpClient());

        private async Task<T> ExecuteAsync<T>(Func<McmaHttpClient, Task<T>> executeAsync) => await executeAsync(await GetMcmaHttpClient());

        public Task PostAsync(object body, string url = null, CancellationToken cancellationToken = default)
            => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.PostAsync(GetFullUrl(url), body, cancellationToken));

        public Task<T> PostAsync<T>(object body, string url = null, CancellationToken cancellationToken = default) where T : McmaObject
            => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.PostAsync<T>(GetFullUrl(url), body, cancellationToken));

        public Task<T> GetAsync<T>(string url = null, CancellationToken cancellationToken = default) where T : McmaObject
            => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.GetAsync<T>(GetFullUrl(url), true, cancellationToken));

        public Task PutAsync(object body, string url = null, CancellationToken cancellationToken = default)
            => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.PutAsync(GetFullUrl(url), body, cancellationToken));

        public Task<T> PutAsync<T>(object body, string url = null, CancellationToken cancellationToken = default) where T : McmaObject
            => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.PutAsync<T>(GetFullUrl(url), body, cancellationToken));

        public Task DeleteAsync(string url = null, CancellationToken cancellationToken = default)
            => ExecuteAsync(async mcmaHttpClient => await mcmaHttpClient.DeleteAsync(GetFullUrl(url), cancellationToken));
    }
}