using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.States;
using Mcma.Worker.Common;
using Microsoft.Extensions.Options;

namespace Mcma.WorkerInvoker.Hangfire;

public class HangfireMcmaWorkerInvoker : McmaWorkerInvoker 
{
    public HangfireMcmaWorkerInvoker(IBackgroundJobClient backgroundJobClient, IOptions<HangfireMcmaWorkerInvokerOptions> options)
    {
        BackgroundJobClient = backgroundJobClient ?? throw new ArgumentNullException(nameof(backgroundJobClient));
        Options = options.Value ?? new HangfireMcmaWorkerInvokerOptions();
    }

    private IBackgroundJobClient BackgroundJobClient { get; }

    private HangfireMcmaWorkerInvokerOptions Options { get; }

    protected override Task InvokeAsync(McmaWorkerRequest workerRequest)
    {
        BackgroundJobClient.Create<IMcmaWorker>(hangfireWorker => hangfireWorker.DoWorkAsync(workerRequest, Guid.NewGuid().ToString()),
                                                new EnqueuedState(Options.QueueName));
        return Task.CompletedTask;
    }
}