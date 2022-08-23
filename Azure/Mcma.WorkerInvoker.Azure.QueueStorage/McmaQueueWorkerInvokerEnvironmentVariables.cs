using Mcma.Utility;

namespace Mcma.WorkerInvoker.Azure.QueueStorage;

public static class McmaQueueWorkerInvokerEnvironmentVariables
{
    public static string WorkerQueueName => McmaEnvironmentVariables.Get("WORKER_QUEUE_NAME", false);
}