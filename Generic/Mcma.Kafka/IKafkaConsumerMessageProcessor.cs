using System.Threading.Tasks;

namespace Mcma.Kafka;

public interface IKafkaConsumerMessageProcessor
{
    Task ProcessAsync(string requestId, string message);
}