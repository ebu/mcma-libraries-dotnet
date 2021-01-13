using System;
using Google.Cloud.Logging.V2;
using Mcma.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.GoogleCloud.Logger
{
    public static class CloudLoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaCloudLogging(this IServiceCollection services, Action<LoggerProviderOptions> configureOptions, Action<LoggingServiceV2ClientBuilder> configureClient = null)
        {
            if (configureOptions != null)
                services.Configure(configureOptions);
            
            var builder = new LoggingServiceV2ClientBuilder();
            configureClient?.Invoke(builder);
            services.AddSingleton(provider => builder.Build());
            
            return services.AddSingleton<ILoggerProvider, CloudLoggingLoggerProvider>();
        }

        public static IServiceCollection AddMcmaCloudLogging(this IServiceCollection services, string source, Action<LoggingServiceV2ClientBuilder> configureClient = null)
            => services.AddMcmaCloudLogging(opts => opts.Source = source, configureClient);
    }
}