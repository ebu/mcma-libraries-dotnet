using Google.Cloud.PubSub.V1;
using Mcma.WorkerInvoker;

namespace Mcma.GoogleCloud.PubSubWorkerInvoker
{
    public class PubSubWorkerInvokerOptions
    {
        public string WorkerTopicName { get; set; } = McmaPubSubWorkerInvokerEnvironmentVariables.WorkerTopicName;

        public PublisherClient.ClientCreationSettings PublisherClientCreationSettings { get; set; }
        
        public PublisherClient.Settings PublisherClientSettings { get; set; }
    }
}