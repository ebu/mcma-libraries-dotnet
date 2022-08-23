using System;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Kafka;

public static class KafkaServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonKafkaProducer<TKey, TMessage>(this IServiceCollection services,
                                                                               Action<ProducerConfig> configure = null)
        => services.AddKafkaProducer<TKey, TMessage>(ServiceLifetime.Singleton, configure);

    public static IServiceCollection AddScopedKafkaProducer<TKey, TMessage>(this IServiceCollection services,
                                                                            Action<ProducerConfig> configure = null)
        => services.AddKafkaProducer<TKey, TMessage>(ServiceLifetime.Scoped, configure);

    public static IServiceCollection AddTransientKafkaProducer<TKey, TMessage>(this IServiceCollection services,
                                                                               Action<ProducerConfig> configure = null)
        => services.AddKafkaProducer<TKey, TMessage>(ServiceLifetime.Transient, configure);

    private static IServiceCollection AddKafkaProducer<TKey, TMessage>(this IServiceCollection services, ServiceLifetime serviceLifetime, Action<ProducerConfig> configure)
    {
        configure ??= KafkaEnvironmentVariables.ApplyDefaults;
        services.Configure(configure);

        services.Add(ServiceDescriptor.Describe(typeof(IProducer<TKey, TMessage>),
                                                svcProvider =>
                                                    new ProducerBuilder<TKey, TMessage>(
                                                            svcProvider.GetService<IOptions<ProducerConfig>>()?.Value ??
                                                            new ProducerConfig())
                                                        .Build(),
                                                serviceLifetime));
        return services;
    }

    public static IServiceCollection AddKafkaConsumer<TKey, TMessage>(this IServiceCollection services, Action<ConsumerConfig> configure = null)
    {
        configure ??= KafkaEnvironmentVariables.ApplyDefaults;
        services.Configure(configure);

        return services.AddSingleton(
            svcProvider =>
                new ConsumerBuilder<TKey, TMessage>(
                        svcProvider.GetService<IOptions<ConsumerConfig>>()?.Value ?? new ConsumerConfig())
                    .Build());
    }

    public static IServiceCollection AddKafkaConsumerService<TProcessor>(this IServiceCollection services,
                                                                         string topic,
                                                                         Action<ConsumerConfig> configureConsumer = null)
        where TProcessor : class, IKafkaConsumerMessageProcessor
        => services.AddKafkaConsumerService<TProcessor>(opts => opts.ConsumerTopic = topic, configureConsumer);

    public static IServiceCollection AddKafkaConsumerService<TProcessor>(this IServiceCollection services,
                                                                         Action<KafkaConsumerServiceOptions> configureService,
                                                                         Action<ConsumerConfig> configureConsumer = null)
        where TProcessor : class, IKafkaConsumerMessageProcessor
        => services.Configure(configureService)
                   .AddKafkaConsumer<string, string>(configureConsumer)
                   .AddSingleton<IKafkaConsumerMessageProcessor, TProcessor>()
                   .AddHostedService<KafkaConsumerService>();
}