using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Logging.Azure.ApplicationInsights;

public static class AppInsightsLoggerProviderServiceCollectionExtensions
{
    public static IServiceCollection AddMcmaAppInsightsLogging(this IServiceCollection services, Action<AppInsightsLoggerProviderOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services.AddMcmaLogging<AppInsightsLoggerProvider>();
    }

    public static IServiceCollection AddMcmaAppInsightsLogging(this IServiceCollection services, string source)
        => services.AddMcmaAppInsightsLogging(opts => opts.Source = source);
}