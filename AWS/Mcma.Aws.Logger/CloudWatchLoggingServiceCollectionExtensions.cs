using System;
using Mcma.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.CloudWatch
{
    public static class CloudWatchLoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaCloudWatchLogging(this IServiceCollection services, Action<CloudWatchLoggerProviderOptions> configureOptions)
        {
            services.Configure(configureOptions);
            return services.AddMcmaLogging<CloudWatchLoggerProvider>();
        }

        public static IServiceCollection AddMcmaCloudWatchLogging(this IServiceCollection services, string source, string logGroupName = null)
            => services.AddMcmaCloudWatchLogging(opts =>
            {
                opts.Source = source;
                
                if (logGroupName != null)
                    opts.LogGroupName = logGroupName;
            });
    }
}