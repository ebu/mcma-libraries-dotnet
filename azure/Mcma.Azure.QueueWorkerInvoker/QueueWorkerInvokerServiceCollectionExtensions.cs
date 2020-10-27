using System;
using Mcma.WorkerInvoker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Azure.WorkerInvoker
{
    public static class QueueWorkerInvokerServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaQueueWorkerInvoker(this IServiceCollection services, Action<QueueWorkerInvokerOptions> configureOptions)
        {
            services.Configure(configureOptions);
            return services.AddSingleton<IWorkerInvoker, QueueWorkerInvoker>();
        }
    }
}