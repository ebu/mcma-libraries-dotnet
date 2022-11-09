using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.Auth;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Aws;

public class Aws4Authenticator : IAuthenticator
{
    public Aws4Authenticator(AuthenticatorKey key, IOptionsSnapshot<Aws4AuthOptions> authOptions)
    {
        var opts = authOptions.Get(key.ToString());
        
        Signer = new Aws4Signer(opts.AccessKey, opts.SecretKey, opts.Region, opts.SessionToken);
    }

    private Aws4Signer Signer { get; }

    public Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        => Signer.SignAsync(request, cancellationToken: cancellationToken);
}