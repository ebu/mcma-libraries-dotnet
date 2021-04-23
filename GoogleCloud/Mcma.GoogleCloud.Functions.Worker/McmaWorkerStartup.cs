using System;
using Google.Cloud.Functions.Hosting;
using Google.Cloud.Storage.V1;
using Mcma.GoogleCloud.Firestore;
using Mcma.GoogleCloud.Storage.Proxies;
using Mcma.Worker;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.GoogleCloud.Functions.Worker
{
    public abstract class McmaWorkerStartup : FunctionsStartup
    {
        protected abstract string ApplicationName { get; }

        protected virtual Action<FirestoreTableOptions> ConfigureFirestoreOptions { get; } = null;

        protected virtual Action<FirestoreTableBuilder> BuildFirestore { get; } = null;
        
        protected virtual Action<CloudStorageClientOptions> ConfigureCloudStorageClientOptions { get; } = null;

        protected virtual Action<StorageClientBuilder> BuildStorageClient { get; } = null;

        protected virtual IServiceCollection ConfigureAdditionalServices(IServiceCollection services) => services;

        protected abstract void BuildWorker(McmaWorkerBuilder workerBuilder);

        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
            => ConfigureAdditionalServices(services)
                .AddMcmaGoogleCloudFunctionWorker(ApplicationName,
                                                  BuildWorker,
                                                  ConfigureFirestoreOptions,
                                                  BuildFirestore,
                                                  ConfigureCloudStorageClientOptions,
                                                  BuildStorageClient);
    }
}