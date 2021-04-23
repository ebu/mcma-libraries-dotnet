using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.States;
using Mcma.Utility;
using Mcma.Worker.Common;
using Mcma.WorkerInvoker;
using Microsoft.Extensions.Options;

namespace Mcma.HangfireWorkerInvoker
{
    public class HangfireMcmaWorkerInvokerOptions
    {
        public string QueueName { get; set; } = McmaEnvironmentVariables.Get("HANGFIRE_QUEUE_NAME", false);
    }

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
}