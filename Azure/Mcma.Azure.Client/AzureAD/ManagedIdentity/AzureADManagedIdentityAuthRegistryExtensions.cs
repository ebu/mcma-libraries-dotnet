﻿using System;
using Mcma.Client;
using Mcma.Client.AccessTokens;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Azure.Client.AzureAD.ManagedIdentity
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