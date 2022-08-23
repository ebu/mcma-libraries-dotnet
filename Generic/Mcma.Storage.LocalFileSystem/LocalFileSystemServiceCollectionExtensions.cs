using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Storage.LocalFileSystem;

public static class LocalFileSystemServiceCollectionExtensions
{
    public static IServiceCollection AddMcmaLocalFileSystemStorageClient(this IServiceCollection services)
    {   
        return services.AddSingletonStorageClient<ILocalFileSystemStorageClient, LocalFileSystemStorageClient>();
    }
}