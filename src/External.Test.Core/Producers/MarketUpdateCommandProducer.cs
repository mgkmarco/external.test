using Confluent.Kafka;
using External.Test.Contracts.Commands;
using External.Test.Contracts.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace External.Test.Producers
{
    public class MarketUpdateCommandProducer : IProducerService<int, UpdateMarketCommand>
    {
        private readonly IProducer<int, UpdateMarketCommand> _producer;
        private readonly ILogger<MarketUpdateCommandProducer> _logger;
        private readonly string _topic;
        private bool _isDisposing;

        public MarketUpdateCommandProducer(IProducer<int, UpdateMarketCommand> producer, ILogger<MarketUpdateCommandProducer> logger)
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _topic = nameof(UpdateMarketCommand);
        }

        public async Task ProduceAsync(int key, UpdateMarketCommand message)
        {
            message.CorrelationId = Guid.NewGuid();
            var kafkaMessage = new Message<int, UpdateMarketCommand>()
            {
                Headers = new Headers{ new Header("CorrelationId", message.CorrelationId.ToByteArray()) },
                Key = key,
                Timestamp = new Timestamp(DateTimeOffset.UtcNow),
                Value = message
            };

            try
            {
                var deliveryReport = await _producer.ProduceAsync(_topic, kafkaMessage);
                _logger.LogInformation($"Partition: {deliveryReport.Partition}, TopicPartition: {deliveryReport.TopicPartition}");
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