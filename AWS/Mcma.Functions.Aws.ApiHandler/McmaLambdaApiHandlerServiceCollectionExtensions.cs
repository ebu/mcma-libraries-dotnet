using System;
using Mcma.Api;
using Mcma.Api.Routing.Defaults;
using Mcma.Api.Aws.ApiGateway;
using Mcma.Api.Http;
using Mcma.Api.Routing.JobAssignments;
using Mcma.Data.Aws.DynamoDb;
using Mcma.Logging.Aws.CloudWatch;
using Mcma.Storage.Aws.S3;
using Mcma.WorkerInvoker.Aws.Lambda;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Functions.Aws.ApiHandler
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