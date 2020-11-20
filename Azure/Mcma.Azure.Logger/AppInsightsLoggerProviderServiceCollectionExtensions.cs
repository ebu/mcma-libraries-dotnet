﻿using System;
using Mcma.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Azure.Logger
{
    public static class AppInsightsLoggerProviderServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaAppInsightsLogging(this IServiceCollection services, Action<AppInsightsLoggerProviderOptions> configureOptions)
        {
            services.Configure(configureOptions);
            return services.AddSingleton<ILoggerProvider, AppInsightsLoggerProvider>();
        }

        public static IServiceCollection AddMcmaAppInsightsLogging(this IServiceCollection services, string source)
            => services.AddMcmaAppInsightsLogging(opts => opts.Source = source);
    }
}