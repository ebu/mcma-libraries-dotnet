using Mcma.Client.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Mcma.Client.Azure.FunctionKeys;

public static class AzureFunctionKeyAuthRegistryExtensions
{
    private static AzureFunctionKeyAuthenticator GetAuthenticator(this IServiceProvider serviceProvider, AzureFunctionKeyAuthenticatorKey key)
        => new(serviceProvider.GetRequiredService<IOptionsSnapshot<AzureFunctionKeyOptions>>().Get(key));

    public static AuthenticatorRegistry AddAzureFunctionKeyAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                string functionKey,
                                                                string serviceName = "",
                                                                string resourceType = "")
    {
        var key = new AzureFunctionKeyAuthenticatorKey(serviceName, resourceType);
        
        authenticatorRegistry.Services.Configure<AzureFunctionKeyOptions>(key.ToString(), o => o.FunctionKey = functionKey);

        return authenticatorRegistry.Add(key, x => x.GetAuthenticator(key));
    }
        
    public static bool TryAddAzureFunctionKeyAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                  string functionKey,
                                                  string serviceName = "",
                                                  string resourceType = "")
    {
        var key = new AzureFunctionKeyAuthenticatorKey(serviceName, resourceType);

        if (!authenticatorRegistry.TryAdd(key, x => x.GetAuthenticator(key)))
            return false;
        
        authenticatorRegistry.Services.Configure<AzureFunctionKeyOptions>(key.ToString(), o => o.FunctionKey = functionKey);

        return true;
    }
}