using System;
using Mcma.Api;
using Mcma.Api.Routing.Defaults;
using Mcma.Aws.ApiGateway;
using Mcma.Aws.CloudWatch;
using Mcma.Aws.DynamoDb;
using Mcma.Aws.S3;
using Mcma.Aws.WorkerInvoker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Functions.ApiHandler
{
    public static class McmaLambdaApiHandlerServiceCollectionExtensions
    {
        static McmaLambdaApiHandlerServiceCollectionExtensions() => S3LocatorHelper.AddTypes();
        
        public static IServiceCollection AddMcmaLambdaApiHandler(this IServiceCollection services,
                                                              string applicationName,
                                                              Action<McmaApiBuilder> buildApi,
                                                              string logGroupName = null,
                                                              Action<DynamoDbTableOptions> configureDynamoDb = null,
                                                              Action<S3StorageClientOptions> configureS3Client = null)
            => services.AddMcmaCloudWatchLogging(applicationName, logGroupName)
                       .AddMcmaDynamoDb(configureDynamoDb)
                       .AddMcmaS3StorageClient(configureS3Client)
                       .AddMcmaApiGatewayApi(buildApi);

        public static IServiceCollection AddMcmaLambdaJobAssignmentApiHandler(this IServiceCollection services,
                                                                              string applicationName,
                                                                              string logGroupName = null,
                                                                              string workerFunctionName = null,
                                                                              Action<DynamoDbTableOptions> configureDynamoDb = null,
                                                                              Action<S3StorageClientOptions> configureS3Client = null,
                                                                              Action<McmaApiBuilder> buildApi = null)
            => services.AddMcmaCloudWatchLogging(applicationName, logGroupName)
                       .AddMcmaDynamoDb(configureDynamoDb)
                       .AddMcmaS3StorageClient(configureS3Client)
                       .AddMcmaLambdaWorkerInvoker(workerFunctionName)
                       .AddMcmaApiGatewayApi(apiBuilder =>
                       {
                           apiBuilder.AddDefaultJobAssignmentRoutes();
                           buildApi?.Invoke(apiBuilder);
                       });
    }
}