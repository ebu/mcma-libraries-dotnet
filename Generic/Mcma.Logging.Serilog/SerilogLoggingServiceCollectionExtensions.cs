using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Mcma.Logging.Serilog
{
    public static class SerilogLoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaSerilogLogging(this IServiceCollection services, Action<LoggerConfiguration> configureSerilog, string source = null)
        {
            if (source != null)
                services.Configure<LoggerProviderOptions>(opts => opts.Source = source);

            var loggerConfig = new LoggerConfiguration();
            configureSerilog(loggerConfig);

            return services.AddSingleton<global::Serilog.ILogger>(_ => loggerConfig.CreateLogger())
                           .AddMcmaLogging<McmaSerilogLoggerProvider>();
        }
    }
}