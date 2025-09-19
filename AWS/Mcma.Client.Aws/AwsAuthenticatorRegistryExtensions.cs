using System;
using Mcma.Client.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Aws;

public static class AwsAuthenticatorRegistryExtensions
{
    private static Aws4Authenticator CreateAuthenticator(this IServiceProvider serviceProvider, Aws4AuthenticatorKey key)
        => new(serviceProvider.GetRequiredService<IOptionsSnapshot<Aws4AuthOptions>>().Get(key));

    private static AuthenticatorRegistry AddAws4Auth(AuthenticatorRegistry authenticatorRegistry,
                                                     Action<IServiceCollection, AuthenticatorKey> configure,
                                                     string serviceName,
                                                     string resourceType)
    {
        var key = new Aws4AuthenticatorKey(serviceName, resourceType);

        configure(authenticatorRegistry.Services, key);

        return authenticatorRegistry.Add(key, x => x.CreateAuthenticator(key));
    }

    public static AuthenticatorRegistry AddAws4Auth(this AuthenticatorRegistry authenticatorRegistry,
                                                    IConfigurationSection configSection,
                                                    string serviceName = "",
                                                    string resourceType = "")
        => AddAws4Auth(authenticatorRegistry,
                       (services, key) => services.Configure<Aws4AuthOptions>(key, configSection),
                       serviceName,
                       resourceType);

    public static AuthenticatorRegistry AddAws4Auth(this AuthenticatorRegistry authenticatorRegistry,
                                                    Action<Aws4AuthOptions> configure,
                                                    string serviceName = "",
                                                    string resourceType = "")
        => AddAws4Auth(authenticatorRegistry,
                       (services, key) => services.Configure(key, configure),
                       serviceName,
                       resourceType);

    public static AuthenticatorRegistry AddAws4AuthForProfile(this AuthenticatorRegistry authenticatorRegistry,
                                                              string profileName,
                                                              string serviceName = "",
                                                              string resourceType = "")
        => authenticatorRegistry.AddAws4Auth(opts => Aws4AuthOptions.ConfigureFromProfile(opts, profileName), serviceName, resourceType);

    public static AuthenticatorRegistry AddAws4AuthForEnvVars(this AuthenticatorRegistry authenticatorRegistry,
                                                              string serviceName = "",
                                                              string resourceType = "")
        => authenticatorRegistry.AddAws4Auth(Aws4AuthOptions.ConfigureFromEnvVars, serviceName, resourceType);

    private static bool TryAddAws4Auth(this AuthenticatorRegistry authenticatorRegistry,
                                       Action<IServiceCollection, AuthenticatorKey> configure,
                                       string serviceName,
                                       string resourceType)
    {
        var key = new Aws4AuthenticatorKey(serviceName, resourceType);

        if (!authenticatorRegistry.TryAdd(key, x => x.CreateAuthenticator(key)))
            return false;

        configure(authenticatorRegistry.Services, key);

        return true;
    }

    public static bool TryAddAws4Auth(this AuthenticatorRegistry authenticatorRegistry,
                                      IConfigurationSection configSection,
                                      string serviceName = "",
                                      string resourceType = "")
        => TryAddAws4Auth(authenticatorRegistry,
                          (services, key) => services.Configure<Aws4AuthOptions>(key, configSection),
                          serviceName,
                          resourceType);

    public static bool TryAddAws4Auth(this AuthenticatorRegistry authenticatorRegistry,
                                      Action<Aws4AuthOptions> configure,
                                      string serviceName = "",
                                      string resourceType = "")
        => TryAddAws4Auth(authenticatorRegistry,
                          (services, key) => services.Configure(key, configure),
                          serviceName,
                          resourceType);

    public static bool TryAddAws4AuthForProfile(this AuthenticatorRegistry authenticatorRegistry,
                                                string profileName,
                                                string serviceName = "",
                                                string resourceType = "")
        => authenticatorRegistry.TryAddAws4Auth(opts => Aws4AuthOptions.ConfigureFromProfile(opts, profileName), serviceName, resourceType);

    public static bool TryAddAws4AuthFromEnvVars(this AuthenticatorRegistry authenticatorRegistry,
                                                 string serviceName = "",
                                                 string resourceType = "")
        => authenticatorRegistry.TryAddAws4Auth(Aws4AuthOptions.ConfigureFromEnvVars, serviceName, resourceType);
}