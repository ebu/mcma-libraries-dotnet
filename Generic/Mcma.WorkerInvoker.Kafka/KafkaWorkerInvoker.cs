using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Mcma.Serialization;
using Mcma.Worker.Common;
using Microsoft.Extensions.Options;

namespace Mcma.WorkerInvoker.Kafka
{
    public class KafkaWorkerInvoker : McmaWorkerInvoker, IDisposable
    {
        public KafkaWorkerInvoker(IProducer<string, string> producer, IOptions<KafkaWorkerInvokerOptions> options)
        {
            Producer = producer ?? throw new ArgumentNullException(nameof(producer));
            
            var opts  = options.Value ?? new KafkaWorkerInvokerOptions();
            if (string.IsNullOrWhiteSpace(opts.WorkerTopic))
                throw new McmaException("No worker topic specified for Kafka worker invoker.");
            
            WorkerTopic = opts.WorkerTopic;
        }
        
        private IProducer<string, string> Producer { get; }
        
        private string WorkerTopic { get; }

        protected override Task InvokeAsync(McmaWorkerRequest workerRequest)
            => Producer.ProduceAsync(WorkerTopic,
                                     new Message<string, string>
                                     {
                                         Key = Guid.NewGuid().ToString(),
                                         Value = workerRequest.ToMcmaJson().ToString()
                                     });

        public void Dispose() => Producer?.Dispose();
    }
}