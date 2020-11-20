﻿using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Logging
{
    public static class LoggingServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MCMA logging using the given the logger provider to the service collection
        /// </summary>
        /// <param name="services">The service collection to add to</param>
        /// <typeparam name="T">The type of <see cref="ILoggerProvider"/> to be added</typeparam>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddMcmaLogging<T>(this IServiceCollection services) where T : class, ILoggerProvider
            => services.AddSingleton<ILoggerProvider, T>();
        
        /// <summary>
        /// Adds MCMA console logging using <see cref="ConsoleLogger"/> to the service collection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMcmaConsoleLogging(this IServiceCollection services)
            => services.AddMcmaLogging<ConsoleLoggerProvider>();
    }
}