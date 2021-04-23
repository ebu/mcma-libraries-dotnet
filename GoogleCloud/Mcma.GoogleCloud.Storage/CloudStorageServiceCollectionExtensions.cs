using System;
using Google.Cloud.Storage.V1;
using Mcma.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.GoogleCloud.Storage.Proxies
{
    public static class CloudStorageServiceCollectionExtensions
    {   
        public static IServiceCollection AddMcmaCloudStorageClient(this IServiceCollection services, Action<CloudStorageClientOptions> configureOptions = null, Action<StorageClientBuilder> build = null)
        {
            var builder = new StorageClientBuilder();
            build?.Invoke(builder);
            services.TryAddSingleton(_ => builder.Build());
            if (configureOptions != null)
                services.Configure((configureOptions));
            services.AddSingletonStorageClient<ICloudStorageClient, CloudStorageClient>();
            return services;
        }
    }
}