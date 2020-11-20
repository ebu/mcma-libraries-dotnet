﻿using System;
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
        static McmaLambdaWorkerCollectionExtensions() => AwsS3LocatorHelper.AddTypes();

        public static IServiceCollection AddMcmaAwsLambdaWorker(this IServiceCollection services,
                                                                string applicationName,
                                                                Action<McmaWorkerBuilder> buildWorker,
                                                                string logGroupName = null,
                                                                Action<DynamoDbTableOptions> configureDynamoDb = null)
            => services.AddMcmaCloudWatchLogging(applicationName, logGroupName)
                       .AddMcmaDynamoDb(configureDynamoDb)
                       .AddMcmaClient(clientBuilder => clientBuilder.Auth.TryAddAws4Auth())
                       .AddMcmaWorker(buildWorker);

        public static IServiceCollection AddMcmaAwsLambdaJobAssignmentWorker<TJob>(this IServiceCollection services,
                                                                                   string applicationName,
                                                                                   Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles,
                                                                                   string logGroupName = null,
                                                                                   Action<DynamoDbTableOptions> configureDynamoDb = null,
                                                                                   Action<McmaWorkerBuilder> addAdditionalOperations = null)
            where TJob : Job
            => services.AddMcmaCloudWatchLogging(applicationName, logGroupName)
                       .AddMcmaDynamoDb(configureDynamoDb)
                       .AddMcmaClient(clientBuilder => clientBuilder.Auth.TryAddAws4Auth())
                       .AddMcmaWorker(workerBuilder =>
                       {
                           workerBuilder.AddProcessJobAssignmentOperation(addProfiles);
                           addAdditionalOperations?.Invoke(workerBuilder);
                       });
    }
}