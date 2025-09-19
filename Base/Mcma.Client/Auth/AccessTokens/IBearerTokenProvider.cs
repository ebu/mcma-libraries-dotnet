#if NET48_OR_GREATER
using System.Net.Http;

#endif
namespace Mcma.Client.Auth.AccessTokens;

public interface IBearerTokenProvider
{
    Task<BearerToken> GetAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}