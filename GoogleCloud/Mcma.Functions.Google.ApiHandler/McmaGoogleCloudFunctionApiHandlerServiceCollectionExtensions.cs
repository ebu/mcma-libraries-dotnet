using System;
using Google.Cloud.Storage.V1;
using Mcma.Api.Google.CloudFunctions;
using Mcma.Api.Http;
using Mcma.Api.Routing.JobAssignments;
using Mcma.Data.Google.Firestore;
using Mcma.Logging.Google.CloudLogging;
using Mcma.Storage.Google.CloudStorage;
using Mcma.WorkerInvoker.Google.PubSub;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Functions.Google.ApiHandler;

public static class McmaGoogleCloudFunctionApiHandlerServiceCollectionExtensions
{
    static McmaGoogleCloudFunctionApiHandlerServiceCollectionExtensions() => CloudStorageLocatorHelper.AddTypes();
        
    public static IServiceCollection AddMcmaGoogleCloudFunctionApiHandler(this IServiceCollection services,
                                                                          string applicationName,
                                                                          Action<McmaApiBuilder> buildApi,
                                                                          Action<FirestoreTableOptions> configureFirestoreOptions = null,
                                                                          Action<FirestoreTableBuilder> buildFirestore = null,
                                                                          Action<CloudStorageClientOptions> configureClientStorageOptions = null,
                                                                          Action<StorageClientBuilder> buildClientStorage = null)
        => services.AddMcmaCloudLogging(applicationName)
                   .AddMcmaFirestore(configureFirestoreOptions, buildFirestore)
                   .AddMcmaCloudStorageClient(configureClientStorageOptions, buildClientStorage)
                   .AddMcmaCloudFunctionApi(buildApi);

    public static IServiceCollection AddMcmaGoogleCloudFunctionJobAssignmentApiHandler(this IServiceCollection services,
                                                                                       string applicationName,
                                                                                       string workerPubSubTopic = null,
                                                                                       Action<FirestoreTableOptions> configureFirestoreOptions = null,
                                                                                       Action<FirestoreTableBuilder> buildFirestore = null,
                                                                                       Action<CloudStorageClientOptions> configureClientStorageOptions = null,
                                                                                       Action<StorageClientBuilder> buildClientStorage = null,
                                                                                       Action<McmaApiBuilder> buildApi = null)
        => services.AddMcmaCloudLogging(applicationName)
                   .AddMcmaFirestore(configureFirestoreOptions, buildFirestore)
                   .AddMcmaCloudStorageClient(configureClientStorageOptions, buildClientStorage)
                   .AddMcmaPubSubWorkerInvoker(workerPubSubTopic)
                   .AddMcmaCloudFunctionApi(apiBuilder =>
                   {
                       apiBuilder.AddDefaultJobAssignmentRoutes();
                       buildApi?.Invoke(apiBuilder);
                   });
}