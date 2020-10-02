using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Mcma.Context;
using Mcma.Serialization;
using Mcma.Utility;
using Mcma.WorkerInvoker;

namespace Mcma.Azure.WorkerInvoker
{
    public class QueueWorkerInvoker : Mcma.WorkerInvoker.WorkerInvoker
    {
        public QueueWorkerInvoker(IContextVariableProvider contextVariableProvider)
            : base(contextVariableProvider)
        {
            QueueServiceClient = new QueueServiceClient(Environment.GetEnvironmentVariable("WEBSITE_CONTENTAZUREFILECONNECTIONSTRING"));
        }
        
        private QueueServiceClient QueueServiceClient { get; }

        protected override async Task InvokeAsync(string workerFunctionId, WorkerRequest request)
        {
            var queueClient = QueueServiceClient.GetQueueClient(workerFunctionId);
            await queueClient.SendMessageAsync(request.ToMcmaJson().ToString().ToBase64());
        }
    }
}
