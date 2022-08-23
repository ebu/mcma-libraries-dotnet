using Mcma.Utility;

namespace Mcma.Worker.Kafka;

public static class KafkaWorkerEnvironmentVariables
{
    public static readonly string WorkerTopic = McmaEnvironmentVariables.Get("KAFKA_WORKER_TOPIC", false);
}