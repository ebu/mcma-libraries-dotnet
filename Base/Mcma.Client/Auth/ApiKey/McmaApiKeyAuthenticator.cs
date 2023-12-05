#if NET48_OR_GREATER
using System.Net.Http;

#endif
namespace Mcma.Client.Auth.ApiKey;

public class McmaApiKeyAuthenticator : IAuthenticator
{
    public const string Header = "x-mcma-api-key";

    public McmaApiKeyAuthenticator(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException($"'{nameof(apiKey)}' cannot be null or whitespace.", nameof(apiKey));

        ApiKey = apiKey;
    }

    private string ApiKey { get; }

    public Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        request.Headers.Add(Header, ApiKey);
        return Task.CompletedTask;
    }
}