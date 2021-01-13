using System;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Mcma.Serialization;
using Mcma.WorkerInvoker;
using Microsoft.Extensions.Options;

namespace Mcma.GoogleCloud.PubSubWorkerInvoker
{
    public class PubSubWorkerInvoker : WorkerInvoker.WorkerInvoker
    {
        public PubSubWorkerInvoker(IOptions<PubSubWorkerInvokerOptions> options)
            : base(options)
        {
            PublisherClientTask =
                new Lazy<Task<PublisherClient>>(
                    () =>
                        PublisherClient.CreateAsync(TopicName.Parse(options.Value.WorkerFunctionId),
                                                    options.Value.PublisherClientCreationSettings,
                                                    options.Value.PublisherClientSettings));
        }

        private Lazy<Task<PublisherClient>> PublisherClientTask { get; }

        protected override async Task InvokeAsync(WorkerRequest workerRequest)
        {
            var publisherClient = await PublisherClientTask.Value;

            await publisherClient.PublishAsync(ByteString.CopyFromUtf8(workerRequest.ToMcmaJson().ToString()));
        }
    }
}