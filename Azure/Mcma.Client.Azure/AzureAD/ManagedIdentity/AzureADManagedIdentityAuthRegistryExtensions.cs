using System;
using Mcma.Client;
using Mcma.Client.Auth;
using Mcma.Client.Auth.AccessTokens;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client.Azure.AzureAD.ManagedIdentity
{
    public static class AzureADManagedIdentityAuthRegistryExtensions
    {
        public static AuthenticatorRegistry AddAzureADManagedIdentityAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                          Action<AzureManagedIdentityBearerTokenProviderOptions> configureOptions = null)
        {
            if (configureOptions != null)
                authenticatorRegistry.Services.Configure(configureOptions);

            return authenticatorRegistry.AddBearerTokens<AzureADAuthContext, AzureManagedIdentityBearerTokenProvider>(AzureConstants.AzureAdAuthType);
        }

        public static AuthenticatorRegistry TryAddAzureADManagedIdentityAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                             Action<AzureManagedIdentityBearerTokenProviderOptions> configureOptions = null)
        {
            if (configureOptions != null)
                authenticatorRegistry.Services.Configure(configureOptions);

            return authenticatorRegistry.TryAddBearerTokens<AzureADAuthContext, AzureManagedIdentityBearerTokenProvider>(
                AzureConstants.AzureAdAuthType);
        }
    }
}