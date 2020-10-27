using Azure.Storage.Queues;
using Mcma.WorkerInvoker;

namespace Mcma.Azure.WorkerInvoker
{
    public class QueueWorkerInvokerOptions : WorkerInvokerOptions
    {
        public string ConnectionString { get; set; }
        
        public QueueClientOptions QueueClient { get; set; }
    }
}