using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client;

public static class McmaClientServiceCollectionExtensions
{
    public static IServiceCollection AddMcmaClient(this IServiceCollection services, ServiceLifetime serviceLifetime, Action<McmaClientBuilder> configure = null)
    {
        var builder = new McmaClientBuilder(services, serviceLifetime);

        configure?.Invoke(builder);

        if (!builder.IsDefaultResourceManagerConfigured)
            builder.AddDefaultResourceManagerFromEnvVars();

        return services;
    }

    public static IServiceCollection AddScopedMcmaClient(this IServiceCollection services, Action<McmaClientBuilder> configure = null)
        => services.AddMcmaClient(ServiceLifetime.Scoped, configure);

    public static IServiceCollection AddSingletonMcmaClient(this IServiceCollection services, Action<McmaClientBuilder> configure = null)
        => services.AddMcmaClient(ServiceLifetime.Singleton, configure);
}