﻿using System;

namespace Mcma.Client
{
    internal interface IAuthenticatorFactoryRegistration
    {
        string AuthType { get; }

        Type FactoryType { get; }
    }
}