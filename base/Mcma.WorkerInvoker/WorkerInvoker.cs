using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Mcma.WorkerInvoker
{
    public abstract class WorkerInvoker : IWorkerInvoker
    {
        protected WorkerInvoker(IOptions<WorkerInvokerOptions> options)
        {
            if (string.IsNullOrWhiteSpace(options.Value?.WorkerFunctionId))
                throw new McmaException("Worker function not configured");

            Options = options.Value;
        }
        
        protected WorkerInvokerOptions Options { get; }
        
        public Task InvokeAsync(string operationName, object input = null, McmaTracker tracker = null)
            =>
                InvokeAsync(
                    new WorkerRequest
                    {
                        OperationName = operationName,
                        Input = input,
                        Tracker = tracker
                    }
                );

        protected abstract Task InvokeAsync(WorkerRequest workerRequest);
    }
}