using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client;

public static class ResourceManagerServiceCollectionExtensions
{
    internal static ResourceManagerBuilder GetResourceManagerBuilder(this IServiceCollection services, string name = null) => new(services, name);

    public static IServiceCollection AddResourceManager(this IServiceCollection services, string name, Action<ResourceManagerBuilder> configure = null)
    {
        var builder = services.GetResourceManagerBuilder(name);
        configure(builder);
        return services;
    }

    public static IServiceCollection AddResourceManager(this IServiceCollection services, Action<ResourceManagerBuilder> configure = null)
        => services.AddResourceManager(Options.DefaultName, configure);
}
