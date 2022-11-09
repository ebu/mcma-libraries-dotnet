using System;
using Mcma.Client.Auth;
using Mcma.Client.Auth.AccessTokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Mcma.Client.Azure.AzureAD.ConfidentialClient;

public static class AzureADConfidentialClientAuthRegistryExtensions
{
    private static void ConfigureOptions(IServiceCollection services,
                                         AuthenticatorKey key,
                                         string[] scopes,
                                         Action<ConfidentialClientApplicationOptions> configureOptions)
    {
        services.Configure(key.ToString(), configureOptions ?? (_ => { }));
        services.Configure<AzureADConfidentialClientApplicationOptions>(key.ToString(), o => o.Scopes = scopes);
    }

    public static AuthenticatorRegistry AddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                         string[] scopes,
                                                                         Action<ConfidentialClientApplicationOptions> configureOptions = null,
                                                                         string serviceName = "",
                                                                         string resourceType = "")
    {
        var key = new AuthenticatorKey(AzureConstants.AzureAdAuthType, serviceName, resourceType);

        ConfigureOptions(authenticatorRegistry.Services, key, scopes, configureOptions);

        return authenticatorRegistry.AddBearerTokens(
            key,
            svcProvider => new AzureADConfidentialClientBearerTokenProvider(
                key,
                svcProvider.GetRequiredService<IOptionsSnapshot<AzureADConfidentialClientApplicationOptions>>()));
    }

    public static bool TryAddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                           string[] scopes,
                                                           Action<ConfidentialClientApplicationOptions> configureOptions = null,
                                                           string serviceName = "",
                                                           string resourceType = "")
    {
        var key = new AuthenticatorKey(AzureConstants.AzureAdAuthType, serviceName, resourceType);

        var added =
            authenticatorRegistry.TryAddBearerTokens(
                key,
                svcProvider => new AzureADConfidentialClientBearerTokenProvider(
                    key,
                    svcProvider.GetRequiredService<IOptionsSnapshot<AzureADConfidentialClientApplicationOptions>>()));

        if (!added)
            return false;

        ConfigureOptions(authenticatorRegistry.Services, key, scopes, configureOptions);

        return true;
    }
}