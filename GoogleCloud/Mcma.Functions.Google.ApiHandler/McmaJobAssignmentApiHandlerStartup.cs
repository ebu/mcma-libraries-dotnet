using System;
using Google.Cloud.Functions.Hosting;
using Google.Cloud.Storage.V1;
using Mcma.Api.Http;
using Mcma.Data.Google.Firestore;
using Mcma.Storage.Google.CloudStorage;
using Mcma.WorkerInvoker.Google.PubSub;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Functions.Google.ApiHandler;

public abstract class McmaJobAssignmentApiHandlerStartup : FunctionsStartup
{
    public abstract string ApplicationName { get; }

    public virtual string WorkerPubSubTopic => McmaPubSubWorkerInvokerEnvironmentVariables.WorkerTopicName;

    protected virtual Action<FirestoreTableOptions> ConfigureFirestoreOptions { get; } = null;

    protected virtual Action<FirestoreTableBuilder> BuildFirestore { get; } = null;
        
    protected virtual Action<CloudStorageClientOptions> ConfigureCloudStorageClientOptions { get; } = null;

    protected virtual Action<StorageClientBuilder> BuildStorageClient { get; } = null;

    protected virtual IServiceCollection ConfigureAdditionalServices(IServiceCollection services) => services;

    protected virtual void AddAdditionalRoutes(McmaApiBuilder builder)
    {
    }

    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        => ConfigureAdditionalServices(services)
            .AddMcmaGoogleCloudFunctionJobAssignmentApiHandler(ApplicationName,
                                                               WorkerPubSubTopic,
                                                               ConfigureFirestoreOptions,
                                                               BuildFirestore,
                                                               ConfigureCloudStorageClientOptions,
                                                               BuildStorageClient,
                                                               AddAdditionalRoutes);
}