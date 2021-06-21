namespace Mcma.WorkerInvoker.Kafka
{
    public class KafkaWorkerInvokerOptions
    {
        public string WorkerTopic { get; set; } = KafkaWorkerInvokerEnvironmentVariables.WorkerTopic;
    }
}