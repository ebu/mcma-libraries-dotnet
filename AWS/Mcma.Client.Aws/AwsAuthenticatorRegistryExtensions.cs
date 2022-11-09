using System;
using Mcma.Client.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Aws;

public static class AwsAuthenticatorRegistryExtensions
{
    public static AuthenticatorRegistry AddAws4Auth(this AuthenticatorRegistry authenticatorRegistry,
                                                    Action<Aws4AuthOptions> configureOptions = null,
                                                    string serviceName = "",
                                                    string resourceType = "")
    {
        var key = new AuthenticatorKey(AwsConstants.Aws4, serviceName, resourceType);

        authenticatorRegistry.Services.Configure(key.ToString(), configureOptions ?? (_ => { }));
            
        return authenticatorRegistry.Add(
            key,
            svcProvider => new Aws4Authenticator(key, svcProvider.GetRequiredService<IOptionsSnapshot<Aws4AuthOptions>>()));
    }

    public static bool TryAddAws4Auth(this AuthenticatorRegistry authenticatorRegistry,
                                      Action<Aws4AuthOptions> configureOptions = null,
                                      string serviceName = "",
                                      string resourceType = "")
    {
        var key = new AuthenticatorKey(AwsConstants.Aws4, serviceName, resourceType);

        if (!authenticatorRegistry.TryAdd<Aws4Authenticator>(key))
            return false;

        authenticatorRegistry.Services.Configure(key.ToString(), configureOptions ?? (_ => { }));

        return true;
    }
}