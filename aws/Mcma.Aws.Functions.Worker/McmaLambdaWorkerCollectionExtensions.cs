using System;
using Mcma.Aws.CloudWatch;
using Mcma.Aws.DynamoDb;
using Mcma.Utility;
using Mcma.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Functions.ApiHandler
{
    public static class McmaLambdaWorkerCollectionExtensions
    {
        public static IServiceCollection AddMcmaAwsLambdaWorker(this IServiceCollection services,
                                                          string applicationName,
                                                          Action<McmaWorkerBuilder> buildWorker)
            => services.AddMcmaCloudWatchLogging(applicationName, McmaEnvironmentVariables.Get("LOG_GROUP_NAME"))
                       .AddMcmaDynamoDb()
                       .AddMcmaWorker(buildWorker);

        public static IServiceCollection AddMcmaAwsLambdaJobAssignmentWorker<TJob>(this IServiceCollection services,
                                                                                   string applicationName,
                                                                                   Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles)
            where TJob : Job
            => services.AddMcmaCloudWatchLogging(applicationName, McmaEnvironmentVariables.Get("LOG_GROUP_NAME"))
                       .AddMcmaDynamoDb()
                       .AddMcmaWorker(workerBuilder => workerBuilder.AddProcessJobAssignmentOperation(addProfiles));
    }
}