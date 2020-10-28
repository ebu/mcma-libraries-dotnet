using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client
{
    public static class McmaClientServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaClient(this IServiceCollection services, Action<AuthenticatorRegistry> addAuth)
        {
            services.AddHttpClient()
                    .AddSingleton<IResourceManagerProvider, ResourceManagerProvider>()
                    .AddSingleton(provider => provider.GetRequiredService<IResourceManagerProvider>().Get());

            return services.AddMcmaAuthentication(addAuth);
        }
    }
}