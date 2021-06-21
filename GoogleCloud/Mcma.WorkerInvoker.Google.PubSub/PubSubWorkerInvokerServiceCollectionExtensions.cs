using System;
using Mcma.WorkerInvoker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.WorkerInvoker.Google.PubSub
{
    public static class PubSubWorkerInvokerServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaPubSubWorkerInvoker(this IServiceCollection services, Action<PubSubWorkerInvokerOptions> configureOptions = null)
        {
            if (configureOptions != null)
                services.Configure(configureOptions);

            return services.AddSingleton<IMcmaWorkerInvoker, PubSubMcmaWorkerInvoker>();
        }

        public static IServiceCollection AddMcmaPubSubWorkerInvoker(this IServiceCollection services, string pubSubTopicName)
            => services.AddMcmaPubSubWorkerInvoker(opts => opts.WorkerTopicName = pubSubTopicName);
    }
}