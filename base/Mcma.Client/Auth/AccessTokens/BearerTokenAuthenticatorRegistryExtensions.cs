using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client.AccessTokens
{
    public static class BearerTokenAuthenticatorRegistryExtensions
    {
        public static AuthenticatorRegistry AddBearerTokens<TAuthContext, TBearerTokenProvider>(
            this AuthenticatorRegistry handlerRegistry,
            string authType,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TBearerTokenProvider : IBearerTokenProvider<TAuthContext>
        {
            handlerRegistry.Services.Add(
                ServiceDescriptor.Describe(typeof(IBearerTokenProvider<TAuthContext>), typeof(TBearerTokenProvider), lifetime));
            
            return handlerRegistry.Add<TAuthContext, BearerTokenAuthenticatorFactory<TAuthContext>>(authType);
        }
    }
}