using System.Threading.Tasks;
using Azure.Storage.Queues;
using Mcma.Serialization;
using Mcma.Utility;
using Mcma.WorkerInvoker;
using Microsoft.Extensions.Options;

namespace Mcma.Azure.WorkerInvoker
{
    public class QueueWorkerInvoker : Mcma.WorkerInvoker.WorkerInvoker
    {
        public QueueWorkerInvoker(IOptions<QueueWorkerInvokerOptions> options)
            : base(options)
        {
            QueueServiceClient = new QueueServiceClient(options.Value.ConnectionString, options.Value.QueueClient);
        }
        
        private QueueServiceClient QueueServiceClient { get; }

        protected override async Task InvokeAsync(string workerFunctionId, WorkerRequest request)
        {
            var queueClient = QueueServiceClient.GetQueueClient(workerFunctionId);
            await queueClient.SendMessageAsync(request.ToMcmaJson().ToString().ToBase64());
        }
    }
}
