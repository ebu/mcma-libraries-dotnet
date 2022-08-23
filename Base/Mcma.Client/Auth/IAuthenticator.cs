using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mcma.Client.Auth;

public interface IAuthenticator
{
    Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}