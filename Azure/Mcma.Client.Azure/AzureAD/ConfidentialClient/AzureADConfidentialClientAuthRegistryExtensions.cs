using System;
using Mcma.Client;
using Mcma.Client.Auth;
using Mcma.Client.Auth.AccessTokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace Mcma.Client.Azure.AzureAD.ConfidentialClient
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
