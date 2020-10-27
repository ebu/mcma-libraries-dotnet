using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Utility
{
    public static class ServiceCollectionExtensions
    {
        public static bool HasService<T>(this IServiceCollection services)
            => services.Any(s => s.ServiceType == typeof(T));
    }
}