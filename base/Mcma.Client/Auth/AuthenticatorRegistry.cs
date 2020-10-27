﻿using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client
{
    public class AuthenticatorRegistry
    {
        internal AuthenticatorRegistry(IServiceCollection services)
        {
            Services = services;
        }
        
        public IServiceCollection Services { get; }

        public AuthenticatorRegistry Add<TAuthContext, TAuthHandlerFactory>(string authType, TAuthContext defaultAuthContext = default)
            where TAuthHandlerFactory : AuthenticatorFactory<TAuthContext>
        {
            Services.AddSingleton<IAuthenticatorFactoryRegistration>(new AuthenticatorFactoryRegistration<TAuthHandlerFactory>(authType, defaultAuthContext)); 
            Services.AddSingleton<IAuthenticatorFactory, TAuthHandlerFactory>();
            return this;
        }
    }
}