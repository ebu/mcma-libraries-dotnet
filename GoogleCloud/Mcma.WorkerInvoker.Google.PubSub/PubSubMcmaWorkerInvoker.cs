using System;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Mcma.Serialization;
using Mcma.Worker.Common;
using Microsoft.Extensions.Options;

namespace Mcma.WorkerInvoker.Google.PubSub;

public class PubSubMcmaWorkerInvoker : McmaWorkerInvoker
{
    public PubSubMcmaWorkerInvoker(IOptions<PubSubWorkerInvokerOptions> options)
    {
        PublisherClientTask =
            new Lazy<Task<PublisherClient>>(
                () =>
                {
                    if (string.IsNullOrWhiteSpace(options?.Value?.WorkerTopicName))
                        throw new McmaException("WorkerTopicName is not set");
                        
                    var publisherClientBuilder = new PublisherClientBuilder
                    {
                        TopicName = TopicName.Parse(options.Value.WorkerTopicName)
                    };
                    
                    options.Value.ConfigurePublisherClient?.Invoke(publisherClientBuilder);

                    return publisherClientBuilder.BuildAsync();
                });
    }

    private Lazy<Task<PublisherClient>> PublisherClientTask { get; }

    protected override async Task InvokeAsync(McmaWorkerRequest workerRequest)
    {
        var publisherClient = await PublisherClientTask.Value;

        await publisherClient.PublishAsync(ByteString.CopyFromUtf8(workerRequest.ToMcmaJson().ToString()));
    }
}