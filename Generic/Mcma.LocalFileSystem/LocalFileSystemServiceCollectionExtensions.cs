using Mcma.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.LocalFileSystem
{
    public static class LocalFileSystemServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaLocalFileSystemStorageClient(this IServiceCollection services)
        {   
            return services.AddSingletonStorageClient<ILocalFileSystemStorageClient, LocalFileSystemStorageClient>();
        }
    }
}