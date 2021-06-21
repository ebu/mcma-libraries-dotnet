using System;
using Mcma.Storage.Azure.BlobStorage;
using Mcma.Client.Azure;
using Mcma.Client.Azure.AzureAD.ManagedIdentity;
using Mcma.Data.Azure.CosmosDb;
using Mcma.Logging.Azure.ApplicationInsights;
using Mcma.Client;
using Mcma.Model.Jobs;
using Mcma.Worker;
using Mcma.Worker.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Functions.Azure.Worker
{
    public static class McmaAzureFunctionWorkerServiceCollectionExtensions
    {
        static McmaAzureFunctionWorkerServiceCollectionExtensions() => BlobStorageLocatorHelper.AddTypes();

        public static IServiceCollection AddMcmaAzureFunctionWorker(this IServiceCollection services,
                                                                    string applicationName,
                                                                    Action<McmaWorkerBuilder> buildWorker,
                                                                    Action<CosmosDbTableOptions> configureCosmosDb = null,
                                                                    Action<BlobStorageClientOptions> configureBlobStorageClient = null)
            =>
                services.AddMcmaAppInsightsLogging(applicationName)
                        .AddMcmaCosmosDb(configureCosmosDb)
                        .AddMcmaBlobStorageClient(configureBlobStorageClient)
                        .AddMcmaClient(clientBuilder => clientBuilder.Auth.TryAddAzureADManagedIdentityAuth())
                        .AddMcmaWorker(buildWorker);

        public static IServiceCollection AddMcmaAzureFunctionJobAssignmentWorker<TJob>(this IServiceCollection services,
                                                                                       string applicationName,
                                                                                       Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles,
                                                                                       Action<CosmosDbTableOptions> configureCosmosDb = null,
                                                                                       Action<BlobStorageClientOptions> configureBlobStorageClient = null,
                                                                                       Action<McmaWorkerBuilder> addAdditionalOperations = null)
            where TJob : Job
            => services.AddMcmaAppInsightsLogging(applicationName)
                       .AddMcmaCosmosDb(configureCosmosDb)
                       .AddMcmaBlobStorageClient(configureBlobStorageClient)
                       .AddMcmaClient(clientBuilder => clientBuilder.Auth.TryAddAzureADManagedIdentityAuth())
                       .AddMcmaWorker(workerBuilder =>
                       {
                           workerBuilder.AddProcessJobAssignmentOperation(addProfiles);
                           addAdditionalOperations?.Invoke(workerBuilder);
                       });
    }
}