using System;
using Mcma.Api.Azure.FunctionApp;
using Mcma.Api.Http;
using Mcma.Api.Routing.JobAssignments;
using Mcma.Logging.Azure.ApplicationInsights;
using Mcma.Data.Azure.CosmosDb;
using Mcma.Storage.Azure.BlobStorage;
using Mcma.WorkerInvoker.Azure.QueueStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Functions.Azure.ApiHandler;

public static class McmaAzureFunctionApiHandlerServiceCollectionExtensions
{
    static McmaAzureFunctionApiHandlerServiceCollectionExtensions() => BlobStorageLocatorHelper.AddTypes();

    public static IServiceCollection AddMcmaAzureFunctionApiHandler(this IServiceCollection services,
                                                                    string applicationName,
                                                                    Action<McmaApiBuilder> buildApi,
                                                                    Action<CosmosDbTableOptions> configureCosmosDb = null,
                                                                    Action<BlobStorageClientOptions> configureBlobStorageClient = null)
        => services.AddMcmaAppInsightsLogging(applicationName)
                   .AddMcmaCosmosDb(configureCosmosDb)
                   .AddMcmaBlobStorageClient(configureBlobStorageClient)
                   .AddMcmaAzureFunctionApi(buildApi);

    public static IServiceCollection AddMcmaAzureFunctionJobAssignmentApiHandler(this IServiceCollection services,
                                                                                 string applicationName,
                                                                                 string workerQueueName = null,
                                                                                 Action<CosmosDbTableOptions> configureCosmosDb = null,
                                                                                 Action<BlobStorageClientOptions> configureBlobStorageClient = null,
                                                                                 Action<McmaApiBuilder> buildApi = null)
        => services.AddMcmaAppInsightsLogging(applicationName)
                   .AddMcmaCosmosDb(configureCosmosDb)
                   .AddMcmaBlobStorageClient(configureBlobStorageClient)
                   .AddMcmaQueueWorkerInvoker(workerQueueName)
                   .AddMcmaAzureFunctionApi(apiBuilder =>
                   {
                       apiBuilder.AddDefaultJobAssignmentRoutes();
                       buildApi?.Invoke(apiBuilder);
                   });
}