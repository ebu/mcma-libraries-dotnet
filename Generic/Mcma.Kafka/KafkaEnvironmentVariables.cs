using System;
using Confluent.Kafka;
using Mcma.Utility;

namespace Mcma.Kafka
{
    public static class KafkaEnvironmentVariables
    {
        public static readonly string BootstrapServers = McmaEnvironmentVariables.Get("KAFKA_BOOTSTRAP_SERVERS", false);
        public static readonly string ConsumerGroupId = McmaEnvironmentVariables.Get("KAFKA_CONSUMER_GROUP", false) ?? "MCMA";
        public static readonly TimeSpan WaitOnSubscribeError =
            TimeSpan.TryParse(McmaEnvironmentVariables.Get("KAFKA_SUBSCRIBE_ERROR_RETRY_INTERVAL", false), out var val) ? val : TimeSpan.FromMinutes(1);
        
        public static ProducerConfig GetDefaultProducerConfig()
        {
            var config = new ProducerConfig();
            ApplyDefaults(config);
            return config;
        }
        
        public static void ApplyDefaults(ConsumerConfig consumerConfig)
        {
            consumerConfig.BootstrapServers = BootstrapServers;
            consumerConfig.GroupId = ConsumerGroupId;
        }
        
        public static ConsumerConfig GetDefaultConsumerConfig()
        {
            var config = new ConsumerConfig();
            ApplyDefaults(config);
            return config;
        }

        public static void ApplyDefaults(ProducerConfig producerConfig)
        {
            producerConfig.BootstrapServers = BootstrapServers;
        }
    }
}