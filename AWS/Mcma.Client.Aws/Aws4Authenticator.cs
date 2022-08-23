using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.Auth;

namespace Mcma.Client.Aws;

public class Aws4Authenticator : IAuthenticator
{
    public Aws4Authenticator(Aws4AuthContext authContext)
    {
        Signer =
            new Aws4Signer(
                authContext.AccessKey,
                authContext.SecretKey,
                authContext.Region,
                authContext.SessionToken);
    }

    private Aws4Signer Signer { get; }

    public Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        => Signer.SignAsync(request, cancellationToken: cancellationToken);
}