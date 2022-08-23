using System;
using Google.Cloud.Functions.Hosting;
using Google.Cloud.Storage.V1;
using Mcma.Data.Google.Firestore;
using Mcma.Model.Jobs;
using Mcma.Storage.Google.CloudStorage;
using Mcma.Worker;
using Mcma.Worker.Jobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Functions.Google.Worker;

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