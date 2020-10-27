using System;
using Mcma.WorkerInvoker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.WorkerInvoker
{
    public static class LambdaWorkerInvokerServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaLambdaWorkerInvoker(this IServiceCollection services, Action<LambdaWorkerInvokerOptions> configureOptions)
        {
            services.Configure(configureOptions);
            return services.AddSingleton<IWorkerInvoker, LambdaWorkerInvoker>();
        }
    }
}