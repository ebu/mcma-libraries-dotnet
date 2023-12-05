#if NET48_OR_GREATER
using System.Net.Http;
#endif

namespace Mcma.Client.Auth;

public interface IAuthenticator
{
    Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}