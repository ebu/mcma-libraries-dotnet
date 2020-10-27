using System;
using Mcma.Azure.Client.AzureAd;
using Mcma.Azure.Client.FunctionKeys;
using Mcma.Client;
using Mcma.Client.AccessTokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace Mcma.Azure.Client
{
    public static class AzureAuthenticatorRegistryExtensions
    {
        public static AuthenticatorRegistry AddAzureFunctionKeyAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                    Action<AzureFunctionKeyAuthenticatorOptions> configureOptions)
        {
            authenticatorRegistry.Services.Configure(configureOptions);
            return authenticatorRegistry.Add<AzureFunctionKeyAuthContext, AzureFunctionKeyAuthenticatorFactory>(AzureConstants.FunctionKeyAuthType);
        }

        public static AuthenticatorRegistry AddAzureADManagedIdentityAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                          Action<AzureManagedIdentityBearerTokenProviderOptions> configureOptions)
        {
            authenticatorRegistry.Services.Configure(configureOptions);
            return authenticatorRegistry.AddBearerTokens<AzureADAuthContext, AzureManagedIdentityBearerTokenProvider>(AzureConstants.AzureAdAuthType);
        }

        public static AuthenticatorRegistry AddAzureADPublicClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                       Action<AzureADPublicClientBearerTokenProviderOptions> configureOptions)
        {
            authenticatorRegistry.Services.Configure(configureOptions);
            return authenticatorRegistry.AddBearerTokens<AzureADAuthContext, AzureADPublicClientBearerTokenProvider>(AzureConstants.AzureAdAuthType);
        }

        public static AuthenticatorRegistry AddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                             Action<ConfidentialClientApplicationOptions> configureOptions)
        {
            authenticatorRegistry.Services.Configure(configureOptions);
            return authenticatorRegistry.AddBearerTokens<AzureADAuthContext, AzureADPublicClientBearerTokenProvider>(AzureConstants.AzureAdAuthType);
        }
    }
}
