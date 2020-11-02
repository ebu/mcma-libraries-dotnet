using System;
using Mcma.Azure.CosmosDb;
using Mcma.Azure.Logger;
using Mcma.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Functions.ApiHandler
{
    public static class McmaAzureFunctionWorkerServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaAzureFunctionWorker(this IServiceCollection services,
                                                                    string applicationName,
                                                                    Action<McmaWorkerBuilder> buildWorker,
                                                                    Action<CosmosDbTableOptions> configureCosmosDb = null)
            =>
                services.AddMcmaAppInsightsLogging(applicationName)
                        .AddMcmaCosmosDb(configureCosmosDb)
                        .AddMcmaWorker(buildWorker);

        public static IServiceCollection AddMcmaAzureFunctionJobAssignmentWorker<TJob>(this IServiceCollection services,
                                                                                       string applicationName,
                                                                                       Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles,
                                                                                       Action<CosmosDbTableOptions> configureCosmosDb = null)
            where TJob : Job
            => services.AddMcmaAppInsightsLogging(applicationName)
                       .AddMcmaCosmosDb(configureCosmosDb)
                       .AddMcmaWorker(workerBuilder => workerBuilder.AddProcessJobAssignmentOperation(addProfiles));
    }
}