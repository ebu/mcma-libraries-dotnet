using System;
using Mcma.Client.Aws;
using Mcma.Data.Aws.DynamoDb;
using Mcma.Storage.Aws.S3;
using Mcma.Client;
using Mcma.Logging.Aws.CloudWatch;
using Mcma.Model.Jobs;
using Mcma.Worker;
using Mcma.Worker.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Functions.Aws.Worker;

public static class McmaLambdaWorkerCollectionExtensions
{
    static McmaLambdaWorkerCollectionExtensions() => S3LocatorHelper.AddTypes();

    public static IServiceCollection AddMcmaAwsLambdaWorker(this IServiceCollection services,
                                                            string applicationName,
                                                            Action<McmaWorkerBuilder> buildWorker,
                                                            string logGroupName = null,
                                                            Action<DynamoDbTableOptions> configureDynamoDb = null,
                                                            Action<S3StorageClientOptions> configureS3Client = null)
        => services.AddMcmaCloudWatchLogging(applicationName, logGroupName)
                   .AddMcmaDynamoDb(configureDynamoDb)
                   .AddMcmaS3StorageClient(configureS3Client)
                   .AddMcmaClient(clientBuilder => clientBuilder.AddAuth(x => x.TryAddAws4AuthFromEnvVars()))
                   .AddMcmaWorker(buildWorker);

    public static IServiceCollection AddMcmaAwsLambdaJobAssignmentWorker<TJob>(this IServiceCollection services,
                                                                               string applicationName,
                                                                               Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles,
                                                                               string logGroupName = null,
                                                                               Action<DynamoDbTableOptions> configureDynamoDb = null,
                                                                               Action<S3StorageClientOptions> configureS3Client = null,
                                                                               Action<McmaWorkerBuilder> addAdditionalOperations = null)
        where TJob : Job
        => services.AddMcmaCloudWatchLogging(applicationName, logGroupName)
                   .AddMcmaDynamoDb(configureDynamoDb)
                   .AddMcmaS3StorageClient(configureS3Client)
                   .AddMcmaClient(clientBuilder => clientBuilder.AddAuth(x => x.TryAddAws4AuthFromEnvVars()))
                   .AddMcmaWorker(workerBuilder =>
                   {
                       workerBuilder.AddProcessJobAssignmentOperation(addProfiles);
                       addAdditionalOperations?.Invoke(workerBuilder);
                   });
}