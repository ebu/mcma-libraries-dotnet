using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client;

public static class McmaClientServiceCollectionExtensions
{
    public static IServiceCollection AddMcmaClient(this IServiceCollection services, Action<McmaClientBuilder> configure = null)
    {
        var builder = new McmaClientBuilder(services);
        configure ??= x => x.AddDefaultResourceManagerFromEnvVars();
        configure(builder);
        return services;
    }
}