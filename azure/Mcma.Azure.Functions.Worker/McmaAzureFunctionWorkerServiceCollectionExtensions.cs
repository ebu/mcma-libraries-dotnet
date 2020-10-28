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
                                                                    Action<McmaWorkerBuilder> buildWorker)
            =>
                services.AddMcmaAppInsightsLogging(applicationName)
                        .AddMcmaCosmosDb(CosmosDbTableProviderOptions.SetFromEnvironmentVariables)
                        .AddMcmaWorker(buildWorker);

        public static IServiceCollection AddMcmaAzureFunctionJobAssignmentWorker<TJob>(this IServiceCollection services,
                                                                                       string applicationName,
                                                                                       Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles)
            where TJob : Job
            => services.AddMcmaAppInsightsLogging(applicationName)
                       .AddMcmaCosmosDb(CosmosDbTableProviderOptions.SetFromEnvironmentVariables)
                       .AddMcmaWorker(workerBuilder => workerBuilder.AddProcessJobAssignmentOperation(addProfiles));
    }
}