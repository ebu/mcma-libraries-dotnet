using System.Threading.Tasks;
using Mcma.Worker.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Mcma.WorkerInvoker
{
    public abstract class McmaWorkerInvoker : IMcmaWorkerInvoker
    {   
        public Task InvokeAsync(string operationName, object input = null, McmaTracker tracker = null)
            =>
                InvokeAsync(
                    new McmaWorkerRequest
                    {
                        OperationName = operationName,
                        Input = input != null ? JObject.FromObject(input) : null,
                        Tracker = tracker
                    }
                );

        protected abstract Task InvokeAsync(McmaWorkerRequest workerRequest);
    }
}