using Mcma.Utility;

namespace Mcma.WorkerInvoker.Kafka;

public static class KafkaWorkerInvokerEnvironmentVariables
{
    public static readonly string WorkerTopic = McmaEnvironmentVariables.Get("KAFKA_WORKER_TOPIC", false);
}