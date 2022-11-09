namespace Mcma.Client.Auth.AccessTokens;

public interface IBearerTokenProvider
{
    Task<BearerToken> GetAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}