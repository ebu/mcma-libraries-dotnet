using System;
using System.Threading.Tasks;
using Mcma.Kafka;
using Mcma.Serialization;
using Mcma.Worker.Common;
using Newtonsoft.Json.Linq;

namespace Mcma.Worker.Kafka;

public class KafkaMcmaWorkerRequestProcessor : IKafkaConsumerMessageProcessor
{
    public KafkaMcmaWorkerRequestProcessor(IMcmaWorker worker) =>
        Worker = worker ?? throw new ArgumentNullException(nameof(worker));
            
    private IMcmaWorker Worker { get; }

    public Task ProcessAsync(string requestId, string message) =>
        Worker.DoWorkAsync(JObject.Parse(message).ToMcmaObject<McmaWorkerRequest>(), requestId);
}