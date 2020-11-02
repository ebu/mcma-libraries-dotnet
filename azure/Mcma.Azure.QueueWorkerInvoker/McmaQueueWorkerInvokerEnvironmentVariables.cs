using Mcma.Utility;

namespace Mcma.Azure.WorkerInvoker
{
    public static class McmaQueueWorkerInvokerEnvironmentVariables
    {
        public static string WorkerQueueName => McmaEnvironmentVariables.Get("WORKER_QUEUE_NAME");
    }
}