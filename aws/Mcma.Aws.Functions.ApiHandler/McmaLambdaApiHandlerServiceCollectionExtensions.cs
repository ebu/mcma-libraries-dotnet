using System;
using Mcma.Api;
using Mcma.Api.Routing.Defaults;
using Mcma.Aws.ApiGateway;
using Mcma.Aws.CloudWatch;
using Mcma.Aws.DynamoDb;
using Mcma.Aws.WorkerInvoker;
using Mcma.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Functions.ApiHandler
{
    public static class McmaLambdaApiHandlerServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaLambdaApiHandler(this IServiceCollection services,
                                                              string applicationName,
                                                              Action<McmaApiBuilder> buildApi)
            => services.AddMcmaCloudWatchLogging(applicationName, McmaEnvironmentVariables.Get("LOG_GROUP_NAME"))
                        .AddMcmaDynamoDb()
                        .AddMcmaApiGatewayApi(buildApi);

        public static IServiceCollection AddMcmaLambdaJobAssignmentApiHandler(this IServiceCollection services, string applicationName)
            => services.AddMcmaCloudWatchLogging(applicationName, McmaEnvironmentVariables.Get("LOG_GROUP_NAME"))
                       .AddMcmaLambdaWorkerInvoker(McmaEnvironmentVariables.Get("WORKER_LAMBDA_FUNCTION_NAME"))
                       .AddMcmaDynamoDb()
                       .AddMcmaApiGatewayApi(apiBuilder => apiBuilder.AddDefaultJobAssignmentRoutes());
    }
}