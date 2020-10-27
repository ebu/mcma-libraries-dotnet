using System;
using Mcma.Aws.CloudWatch;
using Mcma.Aws.DynamoDb;
using Mcma.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.Functions.ApiHandler
{
    public static class WorkerServiceCollectionExtensions
    {
        public static IServiceCollection AddAwsApiHandler(this IServiceCollection services, string applicationName, Action<WorkerBuilder> buildWorker)
        {
            services.AddMcmaCloudWatchLogging(options =>
            {
                options.Source = applicationName;
                options.LogGroupName = Environment.GetEnvironmentVariable(nameof(options.LogGroupName));
            });

            services.AddMcmaDynamoDb();

            services.AddMcmaWorker(buildWorker);

            return services;
        }
        
        public static IServiceCollection AddAwsJobWorker<TJob>(this IServiceCollection services, string applicationName, Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles) where TJob : Job
        {
            services.AddMcmaCloudWatchLogging(options =>
            {
                options.Source = applicationName;
                options.LogGroupName = Environment.GetEnvironmentVariable(nameof(options.LogGroupName));
            });

            services.AddMcmaDynamoDb();

            services.AddMcmaWorker(workerBuilder => workerBuilder.AddProcessJobAssignmentOperation(addProfiles));

            return services;
        }
    }
}