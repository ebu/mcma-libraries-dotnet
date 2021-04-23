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
    public abstract class McmaJobAssignmentWorkerStartup<T> : FunctionsStartup where T : Job
    {
        protected abstract string ApplicationName { get; }

        protected virtual Action<FirestoreTableOptions> ConfigureFirestoreOptions { get; } = null;

        protected virtual Action<FirestoreTableBuilder> BuildFirestore { get; } = null;
        
        protected virtual Action<CloudStorageClientOptions> ConfigureCloudStorageClientOptions { get; } = null;

        protected virtual Action<StorageClientBuilder> BuildStorageClient { get; } = null;

        protected virtual IServiceCollection ConfigureAdditionalServices(IServiceCollection services) => services;

        protected virtual void AddAdditionalOperations(McmaWorkerBuilder builder)
        {
        }

        protected abstract void AddProfiles(ProcessJobAssignmentOperationBuilder<T> builder);

        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
            => ConfigureAdditionalServices(services)
                .AddMcmaGoogleCloudFunctionJobAssignmentWorker<T>(ApplicationName,
                                                                  AddProfiles,
                                                                  ConfigureFirestoreOptions,
                                                                  BuildFirestore,
                                                                  ConfigureCloudStorageClientOptions,
                                                                  BuildStorageClient,
                                                                  AddAdditionalOperations);
    }
}