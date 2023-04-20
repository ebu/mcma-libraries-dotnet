using System;
using Google.Cloud.PubSub.V1;

namespace Mcma.WorkerInvoker.Google.PubSub;

public class PubSubWorkerInvokerOptions
{
    public string WorkerTopicName { get; set; } = McmaPubSubWorkerInvokerEnvironmentVariables.WorkerTopicName;

    public Action<PublisherClientBuilder> ConfigurePublisherClient { get; set; }
}