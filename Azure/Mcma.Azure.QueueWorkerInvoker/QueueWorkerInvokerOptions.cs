using System;
using Azure.Storage.Queues;
using Mcma.WorkerInvoker;

namespace Mcma.Azure.WorkerInvoker
{
    public class QueueWorkerInvokerOptions
    {
        public string WorkerQueueName { get; set; } = McmaQueueWorkerInvokerEnvironmentVariables.WorkerQueueName;

        public string ConnectionString { get; set; } = Environment.GetEnvironmentVariable("WEBSITE_CONTENTAZUREFILECONNECTIONSTRING");
        
        public QueueClientOptions QueueClientOptions { get; set; }
    }
}