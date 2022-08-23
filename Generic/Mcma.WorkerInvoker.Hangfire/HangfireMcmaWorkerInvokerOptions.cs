using Mcma.Utility;

namespace Mcma.WorkerInvoker.Hangfire;

public class HangfireMcmaWorkerInvokerOptions
{
    public string QueueName { get; set; } = McmaEnvironmentVariables.Get("HANGFIRE_QUEUE_NAME", false);
}