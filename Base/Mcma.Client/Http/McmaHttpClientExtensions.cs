using System.Net;
#if NET48_OR_GREATER
using System.Net.Http;
#endif
using Mcma.Serialization;

namespace Mcma.Client.Http;

public static class McmaHttpClientExtensions
{
    public static async Task<HttpResponseMessage> WithErrorHandling(this Task<HttpResponseMessage> responseTask)
        => await (await responseTask).ThrowIfFailedAsync();

    private static async Task<T?> GetAsync<T>(McmaHttpClient mcmaHttpClient, string url, bool nullOn404, CancellationToken cancellationToken)
    {
        var resp = await mcmaHttpClient.GetAsync(url, cancellationToken);
        if (nullOn404 && resp.StatusCode == HttpStatusCode.NotFound)
            return default;

        await resp.ThrowIfFailedAsync();
        var json = await resp.Content.ReadAsJsonAsync();
        return json.ToMcmaObject<T>();
    }

    public static Task<T?> GetWithNullOn404Async<T>(this McmaHttpClient mcmaHttpClient, string url, CancellationToken cancellationToken = default)
        => GetAsync<T>(mcmaHttpClient, url, false, cancellationToken);

    public static async Task<T> GetAsync<T>(this McmaHttpClient mcmaHttpClient, string url, CancellationToken cancellationToken = default)
        => (await GetAsync<T>(mcmaHttpClient, url, true, cancellationToken))!;

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