using System;
using Mcma.Client.Auth;
using Mcma.Client.Auth.AccessTokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Azure.AzureAD.ConfidentialClient;

public static class AzureADConfidentialClientAuthRegistryExtensions
{
    private static AzureADConfidentialClientBearerTokenProvider GetTokenProvider(this IServiceProvider serviceProvider, AuthenticatorKey key)
        => new(serviceProvider.GetRequiredService<IOptionsSnapshot<AzureADConfidentialClientApplicationOptions>>().Get(key));

    private static AuthenticatorRegistry AddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                         Action<IServiceCollection, AuthenticatorKey> configure,
                                                                         string serviceName,
                                                                         string resourceType)
    {
        var key = new AzureADAuthenticatorKey(serviceName, resourceType);

        configure(authenticatorRegistry.Services, key);

        return authenticatorRegistry.AddBearerTokens(key, x => x.GetTokenProvider(key));
    }

    public static AuthenticatorRegistry AddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                         string[] scopes,
                                                                         string serviceName = "",
                                                                         string resourceType = "")
        => authenticatorRegistry.AddAzureADConfidentialClientAuth(
            (services, key) => services.Configure<AzureADConfidentialClientApplicationOptions>(key.ToString(), x => x.Scopes = scopes),
            serviceName,
            resourceType);

    public static AuthenticatorRegistry AddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                         Action<AzureADConfidentialClientApplicationOptions> configureOptions,
                                                                         string serviceName = "",
                                                                         string resourceType = "")
        => authenticatorRegistry.AddAzureADConfidentialClientAuth(
            (services, key) => services.Configure(key.ToString(), configureOptions ?? (_ => { })),
            serviceName,
            resourceType);

    public static AuthenticatorRegistry AddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                         IConfigurationSection configSection,
                                                                         string serviceName = "",
                                                                         string resourceType = "")
        => authenticatorRegistry.AddAzureADConfidentialClientAuth(
            (services, key) => services.Configure<AzureADConfidentialClientApplicationOptions>(key.ToString(), configSection),
            serviceName,
            resourceType);

    public static bool TryAddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                           Action<IServiceCollection, AuthenticatorKey> configure,
                                                           string serviceName,
                                                           string resourceType)
    {
        var key = new AzureADAuthenticatorKey(serviceName, resourceType);

        var added = authenticatorRegistry.TryAddBearerTokens(key, x => x.GetTokenProvider(key));

        if (!added)
            return false;

        configure(authenticatorRegistry.Services, key);

        return true;
    }

    public static bool TryAddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                           string[] scopes,
                                                           string serviceName = "",
                                                           string resourceType = "")
        => authenticatorRegistry.TryAddAzureADConfidentialClientAuth(
            (services, key) => services.Configure<AzureADConfidentialClientApplicationOptions>(key.ToString(), x => x.Scopes = scopes),
            serviceName,
            resourceType);

    public static bool TryAddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                           Action<AzureADConfidentialClientApplicationOptions> configureOptions,
                                                           string serviceName = "",
                                                           string resourceType = "")
        => authenticatorRegistry.TryAddAzureADConfidentialClientAuth(
            (services, key) => services.Configure(key.ToString(), configureOptions ?? (_ => { })),
            serviceName,
            resourceType);

    public static bool TryAddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                           IConfigurationSection configSection,
                                                           string serviceName = "",
                                                           string resourceType = "")
        => authenticatorRegistry.TryAddAzureADConfidentialClientAuth(
            (services, key) => services.Configure<AzureADConfidentialClientApplicationOptions>(key.ToString(), configSection),
            serviceName,
            resourceType);
}