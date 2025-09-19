using System;
using Mcma.Client.Auth;
using Mcma.Client.Auth.AccessTokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Azure.AzureAD.ManagedIdentity;

public static class AzureADManagedIdentityAuthRegistryExtensions
{
    private static AzureADManagedIdentityBearerTokenProvider GetTokenProvider(this IServiceProvider serviceProvider, AuthenticatorKey key)
        => new(serviceProvider.GetRequiredService<IOptionsSnapshot<AzureADManagedIdentityOptions>>().Get(key));

    private static AuthenticatorRegistry AddAzureADManagedIdentityAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                      Action<IServiceCollection, AuthenticatorKey> configure,
                                                                      string serviceName = "",
                                                                      string resourceType = "")
    {
        var key = new AzureADAuthenticatorKey(serviceName, resourceType);

        configure(authenticatorRegistry.Services, key);

        return authenticatorRegistry.AddBearerTokens(key, x => x.GetTokenProvider(key));
    }

    public static AuthenticatorRegistry AddAzureADManagedIdentityAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                      Action<AzureADManagedIdentityOptions> configureOptions,
                                                                      string serviceName = "",
                                                                      string resourceType = "")
        => authenticatorRegistry.AddAzureADManagedIdentityAuth(
            (services, key) => services.Configure(key.ToString(), configureOptions ?? (_ => { })),
            serviceName,
            resourceType);

    public static AuthenticatorRegistry AddAzureADManagedIdentityAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                      string connectionString,
                                                                      string azureAdInstance = null,
                                                                      string serviceName = "",
                                                                      string resourceType = "")
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or whitespace.", nameof(connectionString));

        return authenticatorRegistry.AddAzureADManagedIdentityAuth(
            x =>
            {
                x.ConnectionString = connectionString;
                x.AzureAdInstance = azureAdInstance;
            },
            serviceName,
            resourceType);
    }

    private static bool TryAddAzureADManagedIdentityAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                         Action<IServiceCollection, AuthenticatorKey> configure,
                                                         string serviceName = "",
                                                         string resourceType = "")
    {
        var key = new AzureADAuthenticatorKey(serviceName, resourceType);

        var added = authenticatorRegistry.TryAddBearerTokens(key, x => x.GetTokenProvider(key));

        if (!added)
            return false;

        configure(authenticatorRegistry.Services, key);

        return true;
    }

    public static bool TryAddAzureADManagedIdentityAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                        Action<AzureADManagedIdentityOptions> configureOptions,
                                                        string serviceName = "",
                                                        string resourceType = "")
        => authenticatorRegistry.TryAddAzureADManagedIdentityAuth(
            (services, key) => services.Configure(key.ToString(), configureOptions ?? (_ => { })),
            serviceName,
            resourceType);

    public static bool TryAddAzureADManagedIdentityAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                        string connectionString = null,
                                                        string azureAdInstance = null,
                                                        string serviceName = "",
                                                        string resourceType = "")
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or whitespace.", nameof(connectionString));

        return authenticatorRegistry.TryAddAzureADManagedIdentityAuth(
            x =>
            {
                x.ConnectionString = connectionString;
                x.AzureAdInstance = azureAdInstance;
            },
            serviceName,
            resourceType);
    }
}