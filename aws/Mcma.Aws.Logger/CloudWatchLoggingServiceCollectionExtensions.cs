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
            return services.AddSingleton<ILoggerProvider, CloudWatchLoggerProvider>();
        }

        public static IServiceCollection AddMcmaCloudWatchLogging(this IServiceCollection services, string source, string logGroupName)
            => services.AddMcmaCloudWatchLogging(opts =>
            {
                opts.Source = source;
                opts.LogGroupName = logGroupName;
            });
    }
}