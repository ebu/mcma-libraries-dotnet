using System;
using Mcma.WorkerInvoker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.GoogleCloud.PubSubWorkerInvoker
{
    public static class PubSubWorkerInvokerServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaPubSubWorkerInvoker(this IServiceCollection services, Action<PubSubWorkerInvokerOptions> configureOptions = null)
        {
            if (configureOptions != null)
                services.Configure(configureOptions);

            return services.AddSingleton<IWorkerInvoker, PubSubWorkerInvoker>();
        }

        public static IServiceCollection AddMcmaPubSubWorkerInvoker(this IServiceCollection services, string pubSubTopicName)
            => services.AddMcmaPubSubWorkerInvoker(opts => opts.WorkerFunctionId = pubSubTopicName);
    }
}