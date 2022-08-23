using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client;

public static class McmaClientServiceCollectionExtensions
{
    public static IServiceCollection AddMcmaClient(this IServiceCollection services, Action<McmaClientBuilder> build = null)
    {
        var builder = new McmaClientBuilder(services);
        build?.Invoke(builder);
        return services;
    }
}