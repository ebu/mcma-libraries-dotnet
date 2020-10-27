using System;
using Mcma.Api;
using Mcma.Api.Routing.Defaults;
using Mcma.Aws.ApiGateway;
using Mcma.Aws.CloudWatch;
using Mcma.Aws.DynamoDb;
using Mcma.Aws.WorkerInvoker;
using Mcma.WorkerInvoker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Functions.ApiHandler
{
    public static class ApiHandlerServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaAwsApiHandler(this IServiceCollection services, string applicationName, Action<McmaApiBuilder> buildApi)
        {
            services.AddMcmaCloudWatchLogging(options =>
            {
                options.Source = applicationName;
                options.LogGroupName = Environment.GetEnvironmentVariable(nameof(options.LogGroupName));
            });

            services.AddMcmaDynamoDb();

            services.AddMcmaApiGatewayApi(buildApi);

            return services;
        }
        
        public static IServiceCollection AddMcmaAwsJobAssignmentApiHandler(this IServiceCollection services, string applicationName)
        {
            services.AddMcmaCloudWatchLogging(options =>
            {
                options.Source = applicationName;
                options.LogGroupName = Environment.GetEnvironmentVariable(nameof(options.LogGroupName));
            });

            services.AddMcmaDynamoDb();
            
            services.AddMcmaLambdaWorkerInvoker(opts =>
            {
                opts.WorkerFunctionId = Environment.GetEnvironmentVariable(nameof(WorkerInvokerOptions.WorkerFunctionId));
            });

            services.AddMcmaApiGatewayApi(apiBuilder => apiBuilder.AddDefaultJobAssignmentRoutes());

            return services;
        }
    }
}