using System;
using Mcma.Client.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Azure.FunctionKeys;

public static class AzureFunctionKeyAuthRegistryExtensions
{
    public static AuthenticatorRegistry AddAzureFunctionKeyAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                string functionKey,
                                                                string serviceName = "",
                                                                string resourceType = "")
    {
        var key = new AuthenticatorKey(AzureConstants.FunctionKeyAuthType, serviceName, resourceType);
        
        authenticatorRegistry.Services.Configure<AzureFunctionKeyOptions>(key.ToString(), o => o.FunctionKey = functionKey);

        return authenticatorRegistry.Add(
            key,
            svcProvider => new AzureFunctionKeyAuthenticator(key, svcProvider.GetRequiredService<IOptionsSnapshot<AzureFunctionKeyOptions>>()));
    }
        
    public static bool TryAddAzureFunctionKeyAuth(this AuthenticatorRegistry authenticatorRegistry,
                                                                   string functionKey,
                                                                   string serviceName = "",
                                                                   string resourceType = "")
    {
        var key = new AuthenticatorKey(AzureConstants.FunctionKeyAuthType, serviceName, resourceType);

        var added =
            authenticatorRegistry.TryAdd(
                key,
                svcProvider => new AzureFunctionKeyAuthenticator(key, svcProvider.GetRequiredService<IOptionsSnapshot<AzureFunctionKeyOptions>>()));

        if (!added)
            return false;
        
        authenticatorRegistry.Services.Configure<AzureFunctionKeyOptions>(key.ToString(), o => o.FunctionKey = functionKey);

        return true;
    }
}