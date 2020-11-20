﻿using System;
using Mcma.Api;
using Mcma.Api.Routing.Defaults;
using Mcma.GoogleCloud.Logger;
using Mcma.GoogleCloud.Firestore;
using Mcma.GoogleCloud.HttpFunctionsApi;
using Mcma.GoogleCloud.Storage;
using Mcma.GoogleCloud.PubSubWorkerInvoker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Functions.ApiHandler
{
    public static class McmaGoogleCloudFunctionApiHandlerServiceCollectionExtensions
    {
        static McmaGoogleCloudFunctionApiHandlerServiceCollectionExtensions() => CloudStorageLocatorHelper.AddTypes();
        
        public static IServiceCollection AddMcmaGoogleCloudFunctionApiHandler(this IServiceCollection services,
                                                              string applicationName,
                                                              Action<McmaApiBuilder> buildApi,
                                                              Action<FirestoreTableBuilder> buildFirestore = null)
            => services.AddMcmaCloudLogging(applicationName)
                       .AddMcmaFirestore(build: buildFirestore)
                       .AddMcmaHttpFunctionApi(buildApi);

        public static IServiceCollection AddMcmaGoogleCloudFunctionJobAssignmentApiHandler(this IServiceCollection services,
                                                                              string applicationName,
                                                                              string workerPubSubTopic = null,
                                                                              Action<FirestoreTableBuilder> buildFirestore = null,
                                                                              Action<McmaApiBuilder> buildApi = null)
            => services.AddMcmaCloudLogging(applicationName)
                       .AddMcmaFirestore(build: buildFirestore)
                       .AddMcmaPubSubWorkerInvoker(workerPubSubTopic)
                       .AddMcmaHttpFunctionApi(apiBuilder =>
                       {
                           apiBuilder.AddDefaultJobAssignmentRoutes();
                           buildApi?.Invoke(apiBuilder);
                       });
    }
}