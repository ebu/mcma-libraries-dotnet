using System.Threading.Tasks;
using Mcma.Model;
using Mcma.Serialization;
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
                        Input = input?.ToMcmaJsonObject(),
                        Tracker = tracker
                    }
                );

        protected abstract Task InvokeAsync(McmaWorkerRequest workerRequest);
    }
}