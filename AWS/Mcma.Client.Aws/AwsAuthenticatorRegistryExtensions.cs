using System;
using Mcma.Client.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Aws;

public static class AwsAuthenticatorRegistryExtensions
{
    public static AuthenticatorRegistry AddAws4Auth(this AuthenticatorRegistry authenticatorRegistry,
                                                    Action<Aws4AuthOptions> configureOptions = null,
                                                    string serviceName = "",
                                                    string resourceType = "")
    {
        var key = new AuthenticatorKey(AwsConstants.Aws4, serviceName, resourceType);

        configureOptions ??= Aws4AuthOptions.ConfigureFromEnvironmentVariables;

        authenticatorRegistry.Services.Configure(key, configureOptions);

        return authenticatorRegistry.Add(
            key,
            svcProvider => new Aws4Authenticator(svcProvider.GetRequiredService<IOptionsSnapshot<Aws4AuthOptions>>().Get(key)));
    }
    public static AuthenticatorRegistry AddAws4AuthForProfile(this AuthenticatorRegistry authenticatorRegistry,
                                                              string profileName,
                                                              string serviceName = "",
                                                              string resourceType = "")
        => authenticatorRegistry.AddAws4Auth(opts => Aws4AuthOptions.ConfigureFromProfile(opts, profileName), serviceName, resourceType);

    public static bool TryAddAws4Auth(this AuthenticatorRegistry authenticatorRegistry,
                                      Action<Aws4AuthOptions> configureOptions = null,
                                      string serviceName = "",
                                      string resourceType = "")
    {
        var key = new AuthenticatorKey(AwsConstants.Aws4, serviceName, resourceType);

        if (!authenticatorRegistry.TryAdd<Aws4Authenticator>(key))
            return false;

        authenticatorRegistry.Services.Configure(key.ToString(), configureOptions ?? (_ => { }));

        return true;
    }
}