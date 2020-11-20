﻿using System;
using System.Threading.Tasks;
using Mcma.Client;
using Microsoft.Extensions.Options;

namespace Mcma.Aws.Client
{
    internal class Aws4AuthenticatorFactory : AuthenticatorFactory<Aws4AuthContext>
    {
        public Aws4AuthenticatorFactory(IOptions<Aws4AuthenticatorFactoryOptions> options)
        {
            Options = options.Value ?? new Aws4AuthenticatorFactoryOptions();
        }
        
        private Aws4AuthenticatorFactoryOptions Options { get; }

        protected override Aws4AuthContext DefaultAuthContext => Options.DefaultAuthContext;

        protected override Task<IAuthenticator> GetAsync(Aws4AuthContext authContext)
            => Task.FromResult<IAuthenticator>(new Aws4Authenticator(authContext));
    }
}