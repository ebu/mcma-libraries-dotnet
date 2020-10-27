using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client
{
    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaAuthentication(this IServiceCollection services, Action<AuthenticatorRegistry> addAuthenticators)
        {
            services.AddSingleton<IAuthProvider, AuthProvider>();
            addAuthenticators?.Invoke(new AuthenticatorRegistry(services));
            return services;
        }
    }
}