using System;
using Mcma.Aws.Client;
using Mcma.Aws.CloudWatch;
using Mcma.Aws.DynamoDb;
using Mcma.Aws.S3;
using Mcma.Client;
using Mcma.Serialization;
using Mcma.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Functions.ApiHandler
{
    public static class McmaLambdaWorkerCollectionExtensions
    {
        static McmaLambdaWorkerCollectionExtensions() => McmaTypes.Add<AwsS3FileLocator>().Add<AwsS3FolderLocator>();

        public static IServiceCollection AddMcmaAwsLambdaWorker(this IServiceCollection services,
                                                          string applicationName,
                                                          Action<McmaWorkerBuilder> buildWorker,
                                                          string logGroupName = null)
            => services.AddMcmaCloudWatchLogging(applicationName, logGroupName ?? McmaCloudWatchEnvironmentVariables.LogGroupName)
                       .AddMcmaDynamoDb()
                       .AddMcmaWorker(buildWorker);

        public static IServiceCollection AddMcmaAwsLambdaJobAssignmentWorker<TJob>(this IServiceCollection services,
                                                                                   string applicationName,
                                                                                   Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles,
                                                                                   string logGroupName = null)
            where TJob : Job
            => services.AddMcmaCloudWatchLogging(applicationName, logGroupName ?? McmaCloudWatchEnvironmentVariables.LogGroupName)
                       .AddMcmaDynamoDb()
                       .AddMcmaClient(clientBuilder => clientBuilder.ConfigureDefaultsFromEnvironmentVariables().Auth.AddAws4Auth())
                       .AddMcmaWorker(workerBuilder => workerBuilder.AddProcessJobAssignmentOperation(addProfiles));
    }
}