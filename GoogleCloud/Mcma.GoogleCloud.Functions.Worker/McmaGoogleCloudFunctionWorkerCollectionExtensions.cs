using System;
using Mcma.Client;
using Mcma.GoogleCloud.Client;
using Mcma.GoogleCloud.Logger;
using Mcma.GoogleCloud.Firestore;
using Mcma.GoogleCloud.Storage;
using Mcma.GoogleCloud.Storage.Proxies;
using Mcma.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.GoogleCloud.Functions.Worker
{
    public static class McmaGoogleCloudFunctionWorkerCollectionExtensions
    {
        static McmaGoogleCloudFunctionWorkerCollectionExtensions() => CloudStorageLocatorHelper.AddTypes();

        public static IServiceCollection AddMcmaGoogleCloudFunctionWorker(this IServiceCollection services,
                                                                          string applicationName,
                                                                          Action<McmaWorkerBuilder> buildWorker,
                                                                          Action<FirestoreTableBuilder> buildFirestore = null)
            => services.AddMcmaCloudLogging(applicationName)
                       .AddMcmaFirestore(build: buildFirestore)
                       .AddMcmaClient(clientBuilder => clientBuilder.Auth.TryAddGoogleAuth())
                       .AddMcmaCloudStorage()
                       .AddMcmaWorker(buildWorker);

        public static IServiceCollection AddMcmaGoogleCloudFunctionJobAssignmentWorker<TJob>(this IServiceCollection services,
                                                                                             string applicationName,
                                                                                             Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles,
                                                                                             Action<FirestoreTableBuilder> buildFirestore = null,
                                                                                             Action<McmaWorkerBuilder> addAdditionalOperations = null)
            where TJob : Job
            => services.AddMcmaCloudLogging(applicationName)
                       .AddMcmaFirestore(build: buildFirestore)
                       .AddMcmaClient(clientBuilder => clientBuilder.Auth.TryAddGoogleAuth())
                       .AddMcmaCloudStorage()
                       .AddMcmaWorker(workerBuilder =>
                       {
                           workerBuilder.AddProcessJobAssignmentOperation(addProfiles);
                           addAdditionalOperations?.Invoke(workerBuilder);
                       });
    }
}