using System;

namespace Mcma.Client.Auth
{
    internal class AuthenticatorFactoryRegistration<TAuthenticator> : IAuthenticatorFactoryRegistration
    {
        public AuthenticatorFactoryRegistration(string authType)
        {
            AuthType = authType ?? throw new ArgumentNullException(nameof(authType));
        }

        Type IAuthenticatorFactoryRegistration.FactoryType => typeof(TAuthenticator);
        
        public string AuthType { get; }
    }
}