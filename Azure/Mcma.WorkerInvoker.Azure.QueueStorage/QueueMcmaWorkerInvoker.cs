using System.Threading.Tasks;
using Azure.Storage.Queues;
using Mcma.Serialization;
using Mcma.Utility;
using Mcma.Worker.Common;
using Microsoft.Extensions.Options;

namespace Mcma.WorkerInvoker.Azure.QueueStorage
{
    public class QueueMcmaWorkerInvoker : McmaWorkerInvoker
    {
        public QueueMcmaWorkerInvoker(IOptions<QueueWorkerInvokerOptions> options)
        {
            var queueServiceClient = new QueueServiceClient(options.Value.ConnectionString, options.Value.QueueClientOptions);
            QueueClient = queueServiceClient.GetQueueClient(options.Value.WorkerQueueName);
        }
        
        private QueueClient QueueClient { get; }

        protected override async Task InvokeAsync(McmaWorkerRequest request)
        {
            await QueueClient.SendMessageAsync(request.ToMcmaJson().ToString().ToBase64());
        }
    }
}
