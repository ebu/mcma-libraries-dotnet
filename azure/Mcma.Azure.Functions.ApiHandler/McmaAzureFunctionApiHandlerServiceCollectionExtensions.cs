using System;
using Mcma.Api;
using Mcma.Api.Routing.Defaults;
using Mcma.Azure.CosmosDb;
using Mcma.Azure.FunctionsApi;
using Mcma.Azure.Logger;
using Mcma.Azure.WorkerInvoker;
using Mcma.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Functions.ApiHandler
{
    public static class McmaAzureFunctionApiHandlerServiceCollectionExtensions
    {
        public static IServiceCollection McmaAzureFunctionApiHandler(this IServiceCollection services,
                                                                     string applicationName,
                                                                     Action<McmaApiBuilder> buildApi,
                                                                     Action<CosmosDbTableOptions> configureCosmosDb = null)
            =>
                services.AddMcmaAppInsightsLogging(applicationName)
                        .AddMcmaCosmosDb()
                        .AddMcmaAzureFunctionApi(buildApi);

        public static IServiceCollection McmaAzureFunctionJobAssignmentApiHandler(this IServiceCollection services,
                                                                                  string applicationName,
                                                                                  string workerQueueName = null,
                                                                                  Action<CosmosDbTableOptions> configureCosmosDb = null)
            => services.AddMcmaAppInsightsLogging(applicationName)
                       .AddMcmaCosmosDb(configureCosmosDb)
                       .AddMcmaQueueWorkerInvoker(workerQueueName ?? McmaQueueWorkerInvokerEnvironmentVariables.WorkerQueueName)
                       .AddMcmaAzureFunctionApi(apiBuilder => apiBuilder.AddDefaultJobAssignmentRoutes());
    }
}