using System;

namespace Mcma.Kafka;

public class KafkaConsumerServiceOptions
{
    public string ConsumerTopic { get; set; }
        
    public TimeSpan WaitOnSubscribeError { get; set; } = KafkaEnvironmentVariables.WaitOnSubscribeError;
}