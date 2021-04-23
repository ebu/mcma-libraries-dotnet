using System;
using Mcma.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.S3
{
    public static class S3ServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaS3StorageClient(this IServiceCollection services, Action<S3StorageClientOptions> configureOptions = null)
        {
            if (configureOptions != null)
                services.Configure(configureOptions);
            
            return services.AddSingletonStorageClient<IS3StorageClient, S3StorageClient>();
        }
    }
}