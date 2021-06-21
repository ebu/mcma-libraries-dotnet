using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client.Auth
{
    public class AuthenticatorRegistry
    {
        internal AuthenticatorRegistry(IServiceCollection services)
        {
            Services = services;
        }
        
        public IServiceCollection Services { get; }
        
        private Dictionary<(string, Type), Type> Registrations { get; } = new Dictionary<(string, Type), Type>();

        private AuthenticatorRegistry InternalAdd<TAuthContext, TAuthHandlerFactory>(string authType)
            where TAuthHandlerFactory : AuthenticatorFactory<TAuthContext>
        {
            Services.AddSingleton<IAuthenticatorFactoryRegistration>(new AuthenticatorFactoryRegistration<TAuthHandlerFactory>(authType)); 
            Services.AddSingleton<IAuthenticatorFactory, TAuthHandlerFactory>();
            Registrations[(authType, typeof(TAuthContext))] = typeof(TAuthHandlerFactory);
            return this;
        }

        public AuthenticatorRegistry Add<TAuthContext, TAuthHandlerFactory>(string authType)
            where TAuthHandlerFactory : AuthenticatorFactory<TAuthContext>
        {
            if (Registrations.ContainsKey((authType, typeof(TAuthContext))))
                throw new McmaException(
                    $"An authentication handler for auth type '{authType}' and auth context type '{typeof(TAuthContext)}' has already been registered. " +
                    "If you wish to register a default in the case that no handler was previously registered, please use TryAdd.");
            
            return InternalAdd<TAuthContext, TAuthHandlerFactory>(authType);
        }

        public AuthenticatorRegistry TryAdd<TAuthContext, TAuthHandlerFactory>(string authType)
            where TAuthHandlerFactory : AuthenticatorFactory<TAuthContext>
        {
            return !Registrations.ContainsKey((authType, typeof(TAuthContext)))
                       ? InternalAdd<TAuthContext, TAuthHandlerFactory>(authType)
                       : this;
        }
    }
}