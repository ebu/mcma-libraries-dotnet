﻿using System;
using Mcma.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Client
{
    public static class AwsAuthenticatorRegistryExtensions
    {
        public static AuthenticatorRegistry AddAws4Auth(this AuthenticatorRegistry authenticatorRegistry,
                                                        Action<Aws4AuthenticatorFactoryOptions> configureOptions = null)
        {
            if (configureOptions != null)
                authenticatorRegistry.Services.Configure(configureOptions);
            
            return authenticatorRegistry.Add<Aws4AuthContext, Aws4AuthenticatorFactory>(AwsConstants.Aws4);   
        }
    }
}