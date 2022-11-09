using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Utility;

/// <summary>
/// Utility extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Checks if a service with a given type has been registered
    /// </summary>
    /// <param name="services">The service collection to check</param>
    /// <typeparam name="T">The type to look for</typeparam>
    /// <returns>True if the type has been registered in the service collection; otherwise, false</returns>
    public static bool HasService<T>(this IServiceCollection services)
        => services.Any(s => s.ServiceType == typeof(T));
}