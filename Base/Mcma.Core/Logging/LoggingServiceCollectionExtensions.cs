using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Logging
{
    public static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaLogging<T>(this IServiceCollection services) where T : class, ILoggerProvider
            => services.AddSingleton<ILoggerProvider, T>();
        
        public static IServiceCollection AddMcmaConsoleLogging<T>(this IServiceCollection services) where T : class, ILoggerProvider
            => services.AddSingleton<ILoggerProvider, T>();
    }
}