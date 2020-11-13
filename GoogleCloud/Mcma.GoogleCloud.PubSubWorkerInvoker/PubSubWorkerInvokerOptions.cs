using Google.Cloud.PubSub.V1;
using Mcma.WorkerInvoker;

namespace Mcma.GoogleCloud.PubSubWorkerInvoker
{
    public class PubSubWorkerInvokerOptions : WorkerInvokerOptions
    {
        public PubSubWorkerInvokerOptions()
        {
            WorkerFunctionId = McmaPubSubWorkerInvokerEnvironmentVariables.WorkerTopicName;
        }

        public PublisherClient.ClientCreationSettings PublisherClientCreationSettings { get; set; }
        
        public PublisherClient.Settings PublisherClientSettings { get; set; }
    }
}