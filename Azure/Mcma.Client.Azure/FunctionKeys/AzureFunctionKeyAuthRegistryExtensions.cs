using System;
using Mcma.Client;
using Mcma.Client.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client.Azure.FunctionKeys
{
    public static class AzureFunctionKeyAuthRegistryExtensions
    {
        public static AuthenticatorRegistry AddAzureFunctionKeyAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                    Action<AzureFunctionKeyAuthenticatorOptions> configureOptions = null)
        {
            if (configureOptions != null)
                authenticatorRegistry.Services.Configure(configureOptions);

            return authenticatorRegistry.Add<AzureFunctionKeyAuthContext, AzureFunctionKeyAuthenticatorFactory>(AzureConstants.FunctionKeyAuthType);
        }
        
        public static AuthenticatorRegistry TryAddAzureFunctionKeyAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                       Action<AzureFunctionKeyAuthenticatorOptions> configureOptions = null)
        {
            if (configureOptions != null)
                authenticatorRegistry.Services.Configure(configureOptions);

            return authenticatorRegistry.Add<AzureFunctionKeyAuthContext, AzureFunctionKeyAuthenticatorFactory>(AzureConstants.FunctionKeyAuthType);
        }
    }
}