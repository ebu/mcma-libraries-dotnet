using Mcma.Utility;

namespace Mcma.WorkerInvoker.Google.PubSub
{
    public static class McmaPubSubWorkerInvokerEnvironmentVariables
    {
        public static string WorkerTopicName { get; } = McmaEnvironmentVariables.Get("WORKER_PUBSUB_TOPIC", false);
    }
}