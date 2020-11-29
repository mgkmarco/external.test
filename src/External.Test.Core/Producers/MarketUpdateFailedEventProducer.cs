using Confluent.Kafka;
using External.Test.Contracts.Commands;
using External.Test.Contracts.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace External.Test.Producers
{
    public class MarketUpdateFailedEventProducer : IProducerService<int, UpdateMarketFailedEvent>
    {
        private readonly IProducer<int, UpdateMarketFailedEvent> _producer;
        private readonly ILogger<MarketUpdateFailedEventProducer> _logger;
        private const string _topic = nameof(UpdateMarketFailedEvent);
        private bool _isDisposing;

        public MarketUpdateFailedEventProducer(IProducer<int, UpdateMarketFailedEvent> producer, ILogger<MarketUpdateFailedEventProducer> logger)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ProduceAsync(int key, UpdateMarketFailedEvent message)
        {
            var kafkaMessage = new Message<int, UpdateMarketFailedEvent>()
            {
                Headers = new Headers {new Header("CorrelationId", message.CorrelationId.ToByteArray())},
                Key = key,
                Timestamp = new Timestamp(DateTimeOffset.UtcNow),
                Value = message
            };

            try
            {
                var deliveryReport = await _producer.ProduceAsync(_topic, kafkaMessage);
                _logger.LogInformation(
                    $"Partition: {deliveryReport.Partition}, TopicPartition: {deliveryReport.TopicPartition}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public void Dispose()
        {
            if (_isDisposing)
            {
                return;
            }

            _isDisposing = true;
            _logger.LogInformation("Disposing Kafka Producer [{producerName}] ...", _producer.Name);
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer?.Dispose();
        }
    }
}