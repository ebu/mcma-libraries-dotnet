using System;
using Google.Cloud.Logging.V2;
using Mcma.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Logging.Google.CloudLogging
{
    public static class CloudLoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaCloudLogging(this IServiceCollection services, Action<LoggerProviderOptions> configureOptions, Action<LoggingServiceV2ClientBuilder> configureClient = null)
        {
            if (configureOptions != null)
                services.Configure(configureOptions);
            
            var builder = new LoggingServiceV2ClientBuilder();
            configureClient?.Invoke(builder);
            services.AddSingleton(_ => builder.Build());
            
            return services.AddMcmaLogging<CloudLoggingLoggerProvider>();
        }

        public static IServiceCollection AddMcmaCloudLogging(this IServiceCollection services, string source, Action<LoggingServiceV2ClientBuilder> configureClient = null)
            => services.AddMcmaCloudLogging(opts => opts.Source = source, configureClient);
    }
}