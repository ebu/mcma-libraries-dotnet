using System;
using Mcma.Client;
using Mcma.Client.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client.Google
{
    public static class GoogleAuthenticatorRegistryExtensions
    {
        public static AuthenticatorRegistry AddGoogleAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                          Action<GoogleAuthenticatorOptions> configureOptions = null)
        {
            if (configureOptions != null)
                authenticatorRegistry.Services.Configure(configureOptions);

            return authenticatorRegistry.Add<GoogleAuthContext, GoogleAuthenticatorFactory>(GoogleConstants.AuthType);
        }
        
        public static AuthenticatorRegistry TryAddGoogleAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                             Action<GoogleAuthenticatorOptions> configureOptions = null)
        {
            if (configureOptions != null)
                authenticatorRegistry.Services.Configure(configureOptions);
            
            return authenticatorRegistry.TryAdd<GoogleAuthContext, GoogleAuthenticatorFactory>(GoogleConstants.AuthType);   
        }
    }
}