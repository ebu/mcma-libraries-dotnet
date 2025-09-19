using System;
using Mcma.Client.Auth;
using Mcma.Client.Auth.AccessTokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Google;

public static class GoogleAuthenticatorRegistryExtensions
{
    public static AuthenticatorRegistry AddGoogleAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                      Action<GoogleAuthenticatorOptions> configureOptions = null,
                                                      string serviceName = null,
                                                      string resourceType = null)
    {
        var key = new AuthenticatorKey(GoogleConstants.AuthType, serviceName, resourceType);
        
        authenticatorRegistry.Services.Configure(configureOptions ?? (_ => { }));

        return authenticatorRegistry.AddBearerTokens(
            key,
            svcProvider => new GoogleBearerTokenProvider(key, svcProvider.GetRequiredService<IOptionsSnapshot<GoogleAuthenticatorOptions>>()));
    }

    public static bool TryAddGoogleAuth(this AuthenticatorRegistry authenticatorRegistry,
                                        Action<GoogleAuthenticatorOptions> configureOptions = null,
                                        string serviceName = null,
                                        string resourceType = null)
    {
        var key = new AuthenticatorKey(GoogleConstants.AuthType, serviceName, resourceType);

        var added =
            authenticatorRegistry.TryAddBearerTokens(
                key,
                svcProvider => new GoogleBearerTokenProvider(key, svcProvider.GetRequiredService<IOptionsSnapshot<GoogleAuthenticatorOptions>>()));

        if (!added)
            return false;

        authenticatorRegistry.Services.Configure(configureOptions ?? (_ => { }));

        return true;
    }
}