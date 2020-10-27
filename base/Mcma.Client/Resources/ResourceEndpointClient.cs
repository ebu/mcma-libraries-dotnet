using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mcma.Client
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
            
            var authenticator = AuthProvider != null ? await AuthProvider.GetAsync(AuthType, AuthContext) : null;

            McmaHttpClient = new McmaHttpClient(HttpClient, authenticator, Tracker);

            return McmaHttpClient;
        }

        private string GetFullUrl(string url) => !string.IsNullOrWhiteSpace(url) ? HttpEndpoint?.TrimEnd('/') + "/" + url : HttpEndpoint;

        public async Task<QueryResults<T>> QueryAsync<T>(string url = null,
                                                         IDictionary<string, string> queryParameters = null,
                                                         CancellationToken cancellationToken = default)
            where T : McmaObject
        {
            var mcmaHttpClient = await GetMcmaHttpClient();
            
            url = GetFullUrl(url);
            if (queryParameters != null && queryParameters.Any())
                url += $"?{string.Join("&", queryParameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
            
            return await mcmaHttpClient.GetAsync<QueryResults<T>>(url, cancellationToken);
        }

        public async Task<T> PostAsync<T>(T body, string url = null, CancellationToken cancellationToken = default) where T : McmaObject
        {
            var mcmaHttpClient = await GetMcmaHttpClient();
            
            return await mcmaHttpClient.PostAsync(url, body, cancellationToken);
        }

        public async Task<T> GetAsync<T>(string url = null, CancellationToken cancellationToken = default) where T : McmaObject
        {
            var mcmaHttpClient = await GetMcmaHttpClient();
            
            return await mcmaHttpClient.GetAsync<T>(GetFullUrl(url), cancellationToken);
        }

        public async Task<T> PutAsync<T>(T body, string url = null, CancellationToken cancellationToken = default) where T : McmaObject
        {
            var mcmaHttpClient = await GetMcmaHttpClient();
            
            return await mcmaHttpClient.PostAsync(url, body, cancellationToken);
        }

        public async Task DeleteAsync<T>(string url = null, CancellationToken cancellationToken = default) where T : McmaObject
        {
            var mcmaHttpClient = await GetMcmaHttpClient();
            
            await mcmaHttpClient.DeleteAsync(url, cancellationToken);
        }
    }
}