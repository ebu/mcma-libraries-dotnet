using Mcma.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Serilog
{
    public static class SerilogLoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaSerilogLogging(this IServiceCollection services, string source, global::Serilog.ILogger logger)
        {
            services.Configure<LoggerProviderOptions>(opts => opts.Source = source);

            return services.AddSingleton(logger)
                           .AddMcmaLogging<McmaSerilogLoggerProvider>();
        }
    }
}