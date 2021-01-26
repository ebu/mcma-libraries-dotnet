using System.Collections.Generic;
using System.Threading.Tasks;
using Mcma;

namespace Mcma.WorkerInvoker
{
    public delegate Task InvokeWorker(string workerFunctionId, WorkerRequest request);

    public interface IWorkerInvoker
    {
        Task InvokeAsync(
            string workerFunctionId,
            string operationName,
            object input = null,
            McmaTracker tracker = null);
    }
}
