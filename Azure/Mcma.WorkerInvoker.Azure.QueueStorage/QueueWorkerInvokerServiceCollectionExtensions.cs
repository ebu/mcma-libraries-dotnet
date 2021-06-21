using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.WorkerInvoker.Azure.QueueStorage
{
    public static class QueueWorkerInvokerServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaQueueWorkerInvoker(this IServiceCollection services, Action<QueueWorkerInvokerOptions> configureOptions)
        {
            services.Configure(configureOptions);
            return services.AddSingleton<IMcmaWorkerInvoker, QueueMcmaWorkerInvoker>();
        }

        public static IServiceCollection AddMcmaQueueWorkerInvoker(this IServiceCollection services, string queueName = null)
            => services.AddMcmaQueueWorkerInvoker(
                opts =>
                {
                    if (queueName != null)
                        opts.WorkerQueueName = queueName;
                });
    }
}