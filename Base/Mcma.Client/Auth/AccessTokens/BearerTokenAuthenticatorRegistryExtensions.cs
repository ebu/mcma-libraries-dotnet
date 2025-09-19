using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.Client.Auth.AccessTokens;

public static class BearerTokenAuthenticatorRegistryExtensions
{
    public static AuthenticatorRegistry AddBearerTokens<TBearerTokenProvider>(this AuthenticatorRegistry authRegistry,
                                                                              AuthenticatorKey key,
                                                                              ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TBearerTokenProvider : IBearerTokenProvider
    {
        authRegistry.Services.Add(
            ServiceDescriptor.Describe(typeof(IBearerTokenProvider), typeof(TBearerTokenProvider), lifetime));
            
        return authRegistry.Add<BearerTokenAuthenticator>(key);
    }
    
    public static AuthenticatorRegistry AddBearerTokens<TBearerTokenProvider>(this AuthenticatorRegistry authRegistry,
                                                                              AuthenticatorKey key,
                                                                              Func<IServiceProvider, TBearerTokenProvider> factory,
                                                                              ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TBearerTokenProvider : IBearerTokenProvider
    {
        authRegistry.Services.Add(
            ServiceDescriptor.Describe(typeof(IBearerTokenProvider), svcProvider => factory(svcProvider), lifetime));
            
        return authRegistry.Add<BearerTokenAuthenticator>(key);
    }

    public static bool TryAddBearerTokens<TBearerTokenProvider>(this AuthenticatorRegistry handlerRegistry,
                                                                AuthenticatorKey key,
                                                                ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TBearerTokenProvider : IBearerTokenProvider
    {
        if (!handlerRegistry.TryAdd<BearerTokenAuthenticator>(key))
            return false;
        
        handlerRegistry.Services.TryAdd(
            ServiceDescriptor.Describe(typeof(IBearerTokenProvider), typeof(TBearerTokenProvider), lifetime));

        return true;
    }

    public static bool TryAddBearerTokens<TBearerTokenProvider>(this AuthenticatorRegistry handlerRegistry,
                                                                AuthenticatorKey key,
                                                                Func<IServiceProvider, TBearerTokenProvider> factory,
                                                                ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TBearerTokenProvider : IBearerTokenProvider
    {
        if (!handlerRegistry.TryAdd<BearerTokenAuthenticator>(key))
            return false;
        
        handlerRegistry.Services.TryAdd(
            ServiceDescriptor.Describe(typeof(IBearerTokenProvider), svcProvider => factory(svcProvider), lifetime));

        return true;
    }
}