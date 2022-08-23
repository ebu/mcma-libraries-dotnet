using System;

namespace Mcma.Client.Auth;

internal interface IAuthenticatorFactoryRegistration
{
    string AuthType { get; }

    Type FactoryType { get; }
}