using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Storage;

public static class StorageClientServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonStorageClient<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class, IStorageClient
        where TImplementation : class, TInterface
        => services.AddSingleton<TInterface, TImplementation>()
                   .AddSingleton<IStorageClient>(serviceProvider => serviceProvider.GetRequiredService<TInterface>());
        
    public static IServiceCollection AddScopedStorageClient<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class, IStorageClient
        where TImplementation : class, TInterface
        => services.AddScoped<TInterface, TImplementation>()
                   .AddScoped<IStorageClient>(serviceProvider => serviceProvider.GetRequiredService<TInterface>());
        
    public static IServiceCollection AddTransientStorageClient<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class, IStorageClient
        where TImplementation : class, TInterface
        => services.AddTransient<TInterface, TImplementation>()
                   .AddTransient<IStorageClient>(serviceProvider => serviceProvider.GetRequiredService<TInterface>());
}