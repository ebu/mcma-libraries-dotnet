using Mcma.Utility;

namespace Mcma.GoogleCloud.PubSubWorkerInvoker
{
    public static class McmaPubSubWorkerInvokerEnvironmentVariables
    {
        public static string WorkerTopicName { get; } = McmaEnvironmentVariables.Get("WORKER_PUBSUB_TOPIC", false);
    }
}