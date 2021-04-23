using System;
using Google.Cloud.Storage.V1;
using Mcma.Api;
using Mcma.Api.Routing.Defaults;
using Mcma.GoogleCloud.Logger;
using Mcma.GoogleCloud.Firestore;
using Mcma.GoogleCloud.HttpFunctionsApi;
using Mcma.GoogleCloud.Storage;
using Mcma.GoogleCloud.PubSubWorkerInvoker;
using Mcma.GoogleCloud.Storage.Proxies;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Functions.ApiHandler
{
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
                       .AddMcmaHttpFunctionApi(buildApi);

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
                       .AddMcmaHttpFunctionApi(apiBuilder =>
                       {
                           apiBuilder.AddDefaultJobAssignmentRoutes();
                           buildApi?.Invoke(apiBuilder);
                       });
    }
}