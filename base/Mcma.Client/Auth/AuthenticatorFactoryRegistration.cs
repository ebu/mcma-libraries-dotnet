using System;

namespace Mcma.Client
{
    internal class AuthenticatorFactoryRegistration<TAuthenticator> : IAuthenticatorFactoryRegistration
    {
        public AuthenticatorFactoryRegistration(string authType, object defaultAuthContext = null)
        {
            AuthType = authType ?? throw new ArgumentNullException(nameof(authType));
            DefaultAuthContext = defaultAuthContext;
        }

        Type IAuthenticatorFactoryRegistration.FactoryType => typeof(TAuthenticator);
        
        public string AuthType { get; }

        public object DefaultAuthContext { get; }
    }
}