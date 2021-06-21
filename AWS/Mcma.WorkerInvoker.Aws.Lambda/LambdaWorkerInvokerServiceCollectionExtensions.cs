using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.WorkerInvoker.Aws.Lambda
{
    public static class LambdaWorkerInvokerServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaLambdaWorkerInvoker(this IServiceCollection services, Action<LambdaWorkerInvokerOptions> configureOptions)
        {
            services.Configure(configureOptions);
            return services.AddSingleton<IMcmaWorkerInvoker, LambdaMcmaWorkerInvoker>();
        }

        public static IServiceCollection AddMcmaLambdaWorkerInvoker(this IServiceCollection services, string lambdaFunctionName = null)
            => services.AddMcmaLambdaWorkerInvoker(
                opts =>
                {
                    if (lambdaFunctionName != null)
                        opts.WorkerFunctionName = lambdaFunctionName;
                });
    }
}