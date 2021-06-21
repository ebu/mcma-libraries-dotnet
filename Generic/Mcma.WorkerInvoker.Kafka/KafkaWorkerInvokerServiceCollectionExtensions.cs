using System;
using Confluent.Kafka;
using Mcma.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.WorkerInvoker.Kafka
{
    public static class KafkaWorkerInvokerServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaKafkaWorkerInvoker(this IServiceCollection services, Action<KafkaWorkerInvokerOptions> configure, Action<ProducerConfig> configureProducer)
        {
            services.Configure(configure);
            
            return services.AddSingletonKafkaProducer<string, string>(configureProducer).AddSingleton<IMcmaWorkerInvoker, KafkaWorkerInvoker>();
        }
        
        public static IServiceCollection AddMcmaKafkaWorkerInvoker(this IServiceCollection services, string workerTopic = null, Action<ProducerConfig> configureProducer = null)
        {
            if (workerTopic != null)
                services.Configure<KafkaWorkerInvokerOptions>(opts => opts.WorkerTopic = workerTopic);
            
            return services.AddSingletonKafkaProducer<string, string>(configureProducer).AddSingleton<IMcmaWorkerInvoker, KafkaWorkerInvoker>();
        }
    }
}