using System;
using Confluent.Kafka;
using Mcma.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Worker.Kafka;

public static class KafkaWorkerServiceCollectionExtensions
{
    public static IServiceCollection AddMcmaKafkaWorkerService(this IServiceCollection services,
                                                               Action<KafkaConsumerServiceOptions> configureService,
                                                               Action<ConsumerConfig> configureConsumer = null)
        => services.AddKafkaConsumerService<KafkaMcmaWorkerRequestProcessor>(configureService, configureConsumer);

    public static IServiceCollection AddMcmaKafkaWorkerService(this IServiceCollection services,
                                                               string workerTopic = null,
                                                               Action<ConsumerConfig> configureConsumer = null)
        => services.AddKafkaConsumerService<KafkaMcmaWorkerRequestProcessor>(workerTopic ?? KafkaWorkerEnvironmentVariables.WorkerTopic,
                                                                             configureConsumer);
}