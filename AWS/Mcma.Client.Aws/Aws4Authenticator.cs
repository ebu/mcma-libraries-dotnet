using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.Auth;

namespace Mcma.Client.Aws;

public class Aws4Authenticator : IAuthenticator
{
    public Aws4Authenticator(Aws4AuthOptions opts)
    {
        opts ??= Aws4AuthOptions.CreateFromEnvironmentVariables();

        if (opts is null)
            throw new ArgumentNullException(nameof(opts));

        Signer = new Aws4Signer(opts.AccessKey, opts.SecretKey, opts.Region, opts.SessionToken);
    }

    private Aws4Signer Signer { get; }

    public Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        => Signer.SignAsync(request);
}