using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Mcma.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Mcma.Kafka;

public class KafkaConsumerService : BackgroundService
{
    public KafkaConsumerService(ILoggerProvider loggerProvider,
                                IConsumer<string, string> consumer,
                                IKafkaConsumerMessageProcessor messageProcessor,
                                IOptions<KafkaConsumerServiceOptions> options)
    {
        LoggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
        Consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
        MessageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));

        Options = options.Value ?? new KafkaConsumerServiceOptions();

        if (string.IsNullOrWhiteSpace(Options.ConsumerTopic))
            throw new McmaException("No consumer topic specified for Kafka consumer service.");
    }

    private ILoggerProvider LoggerProvider { get; }

    private IConsumer<string, string> Consumer { get; }

    private IKafkaConsumerMessageProcessor MessageProcessor { get; }

    private KafkaConsumerServiceOptions Options { get; }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
        => Task.Factory.StartNew(() => Consume(cancellationToken),
                                 cancellationToken,
                                 TaskCreationOptions.LongRunning,
                                 TaskScheduler.Default);

    private void Consume(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Consumer.Subscribe(Options.ConsumerTopic);

                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = Consumer.Consume(cancellationToken);
                    var request = result.Message.Value;
                    var requestId = result.Message.Key;

                    var logger = LoggerProvider.Get(requestId);

                    MessageProcessor.ProcessAsync(requestId, request)
                                    .ContinueWith(t =>
                                                  {
                                                      logger.Fatal("An error occurred processing a worker request", t.Exception);
                                                      t.Exception?.Handle(_ => true);
                                                  },
                                                  TaskContinuationOptions.OnlyOnFaulted);
                }
            }
            catch (Exception ex)
            {
                LoggerProvider.Get().Error($"Failed to subscribe to topic {Options.ConsumerTopic}. Retrying in {Options.WaitOnSubscribeError}...",
                                           ex);
                Thread.Sleep(Options.WaitOnSubscribeError);
            }
        }
    }
}