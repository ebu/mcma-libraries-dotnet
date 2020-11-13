using System;
using Mcma.Client;
using Mcma.Client.AccessTokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace Mcma.Azure.Client.AzureAD.ConfidentialClient
{
    public static class AzureADConfidentialClientAuthRegistryExtensions
    {
        public static AuthenticatorRegistry AddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                             Action<ConfidentialClientApplicationOptions> configureOptions = null)
        {
            if (configureOptions != null)
                authenticatorRegistry.Services.Configure(configureOptions);
            
            return authenticatorRegistry.AddBearerTokens<AzureADAuthContext, AzureADConfidentialClientBearerTokenProvider>(AzureConstants.AzureAdAuthType);
        }

        public static AuthenticatorRegistry TryAddAzureADConfidentialClientAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                                Action<ConfidentialClientApplicationOptions> configureOptions = null)
        {
            if (configureOptions != null)
                authenticatorRegistry.Services.Configure(configureOptions);

            return authenticatorRegistry.TryAddBearerTokens<AzureADAuthContext, AzureADConfidentialClientBearerTokenProvider>(AzureConstants.AzureAdAuthType);
        }
    }
}
