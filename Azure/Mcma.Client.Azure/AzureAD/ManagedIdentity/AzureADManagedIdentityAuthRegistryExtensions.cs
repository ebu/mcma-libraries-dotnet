using System;
using Mcma.Client.Auth;
using Mcma.Client.Auth.AccessTokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Azure.AzureAD.ManagedIdentity;

public static class AzureADManagedIdentityAuthRegistryExtensions
{
    public static AuthenticatorRegistry AddAzureADManagedIdentityAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                      Action<AzureADManagedIdentityOptions> configureOptions = null,
                                                                      string serviceName = "",
                                                                      string resourceType = "")
    {
        var key = new AuthenticatorKey(AzureConstants.AzureAdAuthType, serviceName, resourceType);

        authenticatorRegistry.Services.Configure(key.ToString(), configureOptions ?? (_ => { }));

        return authenticatorRegistry.AddBearerTokens(
            key,
            svcProvider =>
                new AzureADManagedIdentityBearerTokenProvider(
                    key,
                    svcProvider.GetRequiredService<IOptionsSnapshot<AzureADManagedIdentityOptions>>()));
    }

    public static bool TryAddAzureADManagedIdentityAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                        Action<AzureADManagedIdentityOptions> configureOptions = null,
                                                        string serviceName = "",
                                                        string resourceType = "")
    {
        var key = new AuthenticatorKey(AzureConstants.AzureAdAuthType, serviceName, resourceType);

        var added =
            authenticatorRegistry.TryAddBearerTokens(
                key,
                svcProvider =>
                    new AzureADManagedIdentityBearerTokenProvider(
                        key,
                        svcProvider.GetRequiredService<IOptionsSnapshot<AzureADManagedIdentityOptions>>()));

        if (!added)
            return false;

        authenticatorRegistry.Services.Configure(key.ToString(), configureOptions ?? (_ => { }));

        return true;
    }
}