﻿using System.Threading.Tasks;
using Mcma.Client.Auth;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Google;

public class GoogleAuthenticatorFactory : AuthenticatorFactory<GoogleAuthContext>
{
    public GoogleAuthenticatorFactory(IOptions<GoogleAuthenticatorOptions> options)
    {
        Options = options.Value ?? new GoogleAuthenticatorOptions();
    }

    private GoogleAuthenticatorOptions Options { get; }

    protected override Task<IAuthenticator> GetAsync(GoogleAuthContext authContext)
        => Task.FromResult<IAuthenticator>(new GoogleAuthenticator(Options, authContext));
}