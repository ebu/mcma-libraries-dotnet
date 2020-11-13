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
            var queueServiceClient = new QueueServiceClient(options.Value.ConnectionString, options.Value.QueueClientOptions);
            QueueClient = queueServiceClient.GetQueueClient(options.Value.WorkerFunctionId);
        }
        
        private QueueClient QueueClient { get; }

        protected override async Task InvokeAsync(WorkerRequest request)
        {
            await QueueClient.SendMessageAsync(request.ToMcmaJson().ToString().ToBase64());
        }
    }
}
