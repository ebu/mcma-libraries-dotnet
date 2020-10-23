using System.Threading.Tasks;

namespace Mcma.WorkerInvoker
{
    public abstract class WorkerInvoker : IWorkerInvoker
    {
        protected WorkerInvoker(IEnvironmentVariables environmentVariables = null)
        {
            EnvironmentVariables = environmentVariables ?? Mcma.EnvironmentVariables.Instance;
        }

        protected IEnvironmentVariables EnvironmentVariables { get; }

        public Task InvokeAsync(string workerFunctionId, string operationName, object input = null, McmaTracker tracker = null)
            =>
            InvokeAsync(
                workerFunctionId,
                new WorkerRequest
                {
                    OperationName = operationName,
                    Input = input,
                    Tracker = tracker
                }
            );

        protected abstract Task InvokeAsync(string workerFunctionId, WorkerRequest workerRequest);
    }
}
