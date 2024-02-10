using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client.Auth;

public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddMcmaAuthentication(this IServiceCollection services, 
                                                           ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
                                                           Action<AuthenticatorRegistry>? addAuthenticators = null)
    {
        services.Add(ServiceDescriptor.Describe(typeof(IAuthProvider), typeof(AuthProvider), serviceLifetime));
        addAuthenticators?.Invoke(new AuthenticatorRegistry(services, serviceLifetime));
        return services;
    }
}