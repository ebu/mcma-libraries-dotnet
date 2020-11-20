﻿using System;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.GoogleCloud.Storage.Proxies
{
    public static class CloudStorageServiceCollectionExtensions
    {   
        public static IServiceCollection AddMcmaCloudStorage(this IServiceCollection services, Action<StorageClientBuilder> build = null)
        {
            var builder = new StorageClientBuilder();
            build?.Invoke(builder);
            services.TryAddSingleton(provider => builder.Build());
            return services;
        }
    }
}