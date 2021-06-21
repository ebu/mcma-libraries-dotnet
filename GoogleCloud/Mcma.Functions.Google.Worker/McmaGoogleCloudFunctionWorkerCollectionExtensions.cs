using System;
using Google.Cloud.Storage.V1;
using Mcma.Client;
using Mcma.Client.Google;
using Mcma.Logging.Google.CloudLogging;
using Mcma.Data.Google.Firestore;
using Mcma.Model.Jobs;
using Mcma.Storage.Google.CloudStorage;
using Mcma.Worker;
using Mcma.Worker.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Functions.Google.Worker
{
    public static class McmaGoogleCloudFunctionWorkerCollectionExtensions
    {
        static McmaGoogleCloudFunctionWorkerCollectionExtensions() => CloudStorageLocatorHelper.AddTypes();

        public static IServiceCollection AddMcmaGoogleCloudFunctionWorker(this IServiceCollection services,
                                                                          string applicationName,
                                                                          Action<McmaWorkerBuilder> buildWorker,
                                                                          Action<FirestoreTableOptions> configureFirestoreOptions = null,
                                                                          Action<FirestoreTableBuilder> buildFirestore = null,
                                                                          Action<CloudStorageClientOptions> configureClientStorageOptions = null,
                                                                          Action<StorageClientBuilder> buildClientStorage = null)
            => services.AddMcmaCloudLogging(applicationName)
                       .AddMcmaFirestore(configureFirestoreOptions, buildFirestore)
                       .AddMcmaCloudStorageClient(configureClientStorageOptions, buildClientStorage)
                       .AddMcmaClient(clientBuilder => clientBuilder.Auth.TryAddGoogleAuth())
                       .AddMcmaCloudStorageClient()
                       .AddMcmaWorker(buildWorker);

        public static IServiceCollection AddMcmaGoogleCloudFunctionJobAssignmentWorker<TJob>(this IServiceCollection services,
                                                                                             string applicationName,
                                                                                             Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles,
                                                                                             Action<FirestoreTableOptions> configureFirestoreOptions = null,
                                                                                             Action<FirestoreTableBuilder> buildFirestore = null,
                                                                                             Action<CloudStorageClientOptions> configureClientStorageOptions = null,
                                                                                             Action<StorageClientBuilder> buildClientStorage = null,
                                                                                             Action<McmaWorkerBuilder> addAdditionalOperations = null)
            where TJob : Job
            => services.AddMcmaCloudLogging(applicationName)
                       .AddMcmaFirestore(configureFirestoreOptions, buildFirestore)
                       .AddMcmaCloudStorageClient(configureClientStorageOptions, buildClientStorage)
                       .AddMcmaClient(clientBuilder => clientBuilder.Auth.TryAddGoogleAuth())
                       .AddMcmaCloudStorageClient()
                       .AddMcmaWorker(workerBuilder =>
                       {
                           workerBuilder.AddProcessJobAssignmentOperation(addProfiles);
                           addAdditionalOperations?.Invoke(workerBuilder);
                       });
    }
}