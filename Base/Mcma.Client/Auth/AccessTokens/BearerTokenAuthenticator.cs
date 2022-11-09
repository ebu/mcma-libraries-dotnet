using System.Net.Http.Headers;

namespace Mcma.Client.Auth.AccessTokens;

internal class BearerTokenAuthenticator : IAuthenticator
{
    public BearerTokenAuthenticator(IBearerTokenProvider bearerTokenProvider)
    {
        BearerTokenProvider = bearerTokenProvider ?? throw new ArgumentNullException(nameof(bearerTokenProvider));
    }

    private IBearerTokenProvider BearerTokenProvider { get; }

    private BearerToken BearerToken { get; set; }

    public async Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        if (BearerToken?.ExpiresOn != null && BearerToken.ExpiresOn >= DateTimeOffset.UtcNow)
            BearerToken = null;

        BearerToken ??= await BearerTokenProvider.GetAsync(request, cancellationToken);

        if (BearerToken != null)
            request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {BearerToken.Token}");
    }
}