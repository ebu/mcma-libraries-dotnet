using System;
using Mcma.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Storage.Azure.BlobStorage
{
    public static class BlobStorageServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaBlobStorageClient(this IServiceCollection services,
                                                                  Action<BlobStorageClientOptions> configureOptions = null)
        {
            if (configureOptions != null)
                services.Configure(configureOptions);

            return services.AddScopedStorageClient<IBlobStorageClient, BlobStorageClient>();
        }
    }
}