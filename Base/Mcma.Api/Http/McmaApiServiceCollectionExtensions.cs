using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Api.Http;

public static class McmaApiServiceCollectionExtensions
{
    public static IServiceCollection AddMcmaApi(this IServiceCollection services, Action<McmaApiBuilder> build)
    {
        var builder = new McmaApiBuilder(services);
        build(builder);
        return services;
    }
}