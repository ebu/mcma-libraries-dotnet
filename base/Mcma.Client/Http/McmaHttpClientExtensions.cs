using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Serialization;

namespace Mcma.Client
{
    public static class McmaHttpClientExtensions
    {
        public static async Task<HttpResponseMessage> WithErrorHandling(this Task<HttpResponseMessage> responseTask)
            => await (await responseTask).ThrowIfFailedAsync();

        public static async Task<T> GetAsync<T>(this McmaHttpClient mcmaHttpClient, string url, bool nullOn404 = false, CancellationToken cancellationToken = default)
        {
            var resp = await mcmaHttpClient.GetAsync(url, cancellationToken);
            if (nullOn404 && resp.StatusCode == HttpStatusCode.NotFound)
                return default;
            
            await resp.ThrowIfFailedAsync();
            var json = await resp.Content.ReadAsJsonAsync();
            return json.ToMcmaObject<T>();
        }

        public static async Task PostAsync(this McmaHttpClient mcmaHttpClient,
                                                                   string url,
                                                                   object body,
                                                                   CancellationToken cancellationToken = default)
        {
            var resp = await mcmaHttpClient.PostAsync(url, new McmaJsonContent(body), cancellationToken);
            await resp.ThrowIfFailedAsync();
        }

        public static async Task<T> PostAsync<T>(this McmaHttpClient mcmaHttpClient,
                                                 string url,
                                                 object body,
                                                 CancellationToken cancellationToken = default)
        {
            var resp = await mcmaHttpClient.PostAsync(url, new McmaJsonContent(body), cancellationToken);
            await resp.ThrowIfFailedAsync();
            var json = await resp.Content.ReadAsJsonAsync();
            return json.ToMcmaObject<T>();
        }

        public static async Task PutAsync(this McmaHttpClient mcmaHttpClient, string url, object body, CancellationToken cancellationToken = default)
        {
            var resp = await mcmaHttpClient.PutAsync(url, new McmaJsonContent(body), cancellationToken);
            await resp.ThrowIfFailedAsync();
        }

        public static async Task<T> PutAsync<T>(this McmaHttpClient mcmaHttpClient, string url, object body, CancellationToken cancellationToken = default)
        {
            var resp = await mcmaHttpClient.PutAsync(url, new McmaJsonContent(body), cancellationToken);
            await resp.ThrowIfFailedAsync();
            var json = await resp.Content.ReadAsJsonAsync();
            return json.ToMcmaObject<T>();
        }
    }
}