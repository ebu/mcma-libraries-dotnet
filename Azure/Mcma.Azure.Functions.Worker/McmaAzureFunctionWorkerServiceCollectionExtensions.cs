using System;
using Mcma.Azure.BlobStorage;
using Mcma.Azure.Client;
using Mcma.Azure.Client.AzureAD.ManagedIdentity;
using Mcma.Azure.CosmosDb;
using Mcma.Azure.Logger;
using Mcma.Client;
using Mcma.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Azure.Functions.Worker
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