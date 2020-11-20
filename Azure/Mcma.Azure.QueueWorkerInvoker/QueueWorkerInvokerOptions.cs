﻿using System;
using Azure.Storage.Queues;
using Mcma.WorkerInvoker;

namespace Mcma.Azure.WorkerInvoker
{
    public class QueueWorkerInvokerOptions : WorkerInvokerOptions
    {
        public QueueWorkerInvokerOptions()
        {
            WorkerFunctionId = McmaQueueWorkerInvokerEnvironmentVariables.WorkerQueueName;
        }

        public string ConnectionString { get; set; } = Environment.GetEnvironmentVariable("WEBSITE_CONTENTAZUREFILECONNECTIONSTRING");
        
        public QueueClientOptions QueueClientOptions { get; set; }
    }
}