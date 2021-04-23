using System;
using Hangfire;
using Mcma.WorkerInvoker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.HangfireWorkerInvoker
{
    public static class HangfireWorkerInvokerServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaHangfireWorkerInvoker(this IServiceCollection services,
                                                                      Action<IGlobalConfiguration> configureHangfire,
                                                                      Action<HangfireMcmaWorkerInvokerOptions> configureWorkerInvoker = null)
        {
            services.AddHangfire(configureHangfire);

            if (configureWorkerInvoker != null)
                services.Configure(configureWorkerInvoker);

            return services.AddSingleton<IMcmaWorkerInvoker, HangfireMcmaWorkerInvoker>();
        }

        public static IServiceCollection AddMcmaHangfireWorkerInvoker(this IServiceCollection services,
                                                                      string queueName,
                                                                      Action<IGlobalConfiguration> configureHangfire)
            => services.AddMcmaHangfireWorkerInvoker(configureHangfire, opts => opts.QueueName = queueName);
    }
}