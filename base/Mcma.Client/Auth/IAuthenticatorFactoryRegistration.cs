using System;

namespace Mcma.Client
{
    internal interface IAuthenticatorFactoryRegistration
    {
        string AuthType { get; }
        
        object DefaultAuthContext { get; }

        Type FactoryType { get; }
    }
}