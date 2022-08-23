using System;
using Azure.Storage.Queues;

namespace Mcma.WorkerInvoker.Azure.QueueStorage;

public class QueueWorkerInvokerOptions
{
    public string WorkerQueueName { get; set; } = McmaQueueWorkerInvokerEnvironmentVariables.WorkerQueueName;

    public string ConnectionString { get; set; } = Environment.GetEnvironmentVariable("WEBSITE_CONTENTAZUREFILECONNECTIONSTRING");
        
    public QueueClientOptions QueueClientOptions { get; set; }
}