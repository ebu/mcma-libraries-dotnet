using System;
using Mcma.Api;
using Mcma.Api.Routing.Defaults;
using Mcma.Aws.ApiGateway;
using Mcma.Aws.CloudWatch;
using Mcma.Aws.DynamoDb;
using Mcma.Aws.S3;
using Mcma.Aws.WorkerInvoker;
using Mcma.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Functions.ApiHandler
{
    public static class McmaLambdaApiHandlerServiceCollectionExtensions
    {
        static McmaLambdaApiHandlerServiceCollectionExtensions() => McmaTypes.Add<AwsS3FileLocator>().Add<AwsS3FolderLocator>();
        
        public static IServiceCollection AddMcmaLambdaApiHandler(this IServiceCollection services,
                                                              string applicationName,
                                                              Action<McmaApiBuilder> buildApi,
                                                              string logGroupName = null,
                                                              Action<DynamoDbTableOptions> configureDynamoDb = null)
            => services.AddMcmaCloudWatchLogging(applicationName, logGroupName)
                       .AddMcmaDynamoDb(configureDynamoDb)
                       .AddMcmaApiGatewayApi(buildApi);

        public static IServiceCollection AddMcmaLambdaJobAssignmentApiHandler(this IServiceCollection services,
                                                                              string applicationName,
                                                                              string logGroupName = null,
                                                                              string workerFunctionName = null,
                                                                              Action<DynamoDbTableOptions> configureDynamoDb = null)
            => services.AddMcmaCloudWatchLogging(applicationName, logGroupName)
                       .AddMcmaLambdaWorkerInvoker(workerFunctionName)
                       .AddMcmaDynamoDb(configureDynamoDb)
                       .AddMcmaApiGatewayApi(apiBuilder => apiBuilder.AddDefaultJobAssignmentRoutes());
    }
}