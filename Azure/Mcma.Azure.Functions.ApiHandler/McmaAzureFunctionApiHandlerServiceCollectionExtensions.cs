﻿using System;
using Mcma.Api;
using Mcma.Api.Routing.Defaults;
using Mcma.Azure.BlobStorage;
using Mcma.Azure.CosmosDb;
using Mcma.Azure.FunctionsApi;
using Mcma.Azure.Logger;
using Mcma.Azure.WorkerInvoker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Azure.Functions.ApiHandler
{
    public static class McmaAzureFunctionApiHandlerServiceCollectionExtensions
    {
        static McmaAzureFunctionApiHandlerServiceCollectionExtensions() => BlobStorageLocatorHelper.AddTypes();

        public static IServiceCollection AddMcmaAzureFunctionApiHandler(this IServiceCollection services,
                                                                        string applicationName,
                                                                        Action<McmaApiBuilder> buildApi,
                                                                        Action<CosmosDbTableOptions> configureCosmosDb = null)
            => services.AddMcmaAppInsightsLogging(applicationName)
                       .AddMcmaCosmosDb(configureCosmosDb)
                       .AddMcmaAzureFunctionApi(buildApi);

        public static IServiceCollection AddMcmaAzureFunctionJobAssignmentApiHandler(this IServiceCollection services,
                                                                                     string applicationName,
                                                                                     string workerQueueName = null,
                                                                                     Action<CosmosDbTableOptions> configureCosmosDb = null,
                                                                                     Action<McmaApiBuilder> buildApi = null)
            => services.AddMcmaAppInsightsLogging(applicationName)
                       .AddMcmaCosmosDb(configureCosmosDb)
                       .AddMcmaQueueWorkerInvoker(workerQueueName)
                       .AddMcmaAzureFunctionApi(apiBuilder =>
                       {
                           apiBuilder.AddDefaultJobAssignmentRoutes();
                           buildApi?.Invoke(apiBuilder);
                       });
    }
}