using AutoMapper;
using Confluent.Kafka;
using External.Test.Contracts.Commands;
using External.Test.Contracts.Services;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace External.Test.Consumers
{
    public class MarketUpdateCommandConsumer : BackgroundService
    {
        private readonly IConsumer<int, UpdateMarketCommand> _consumer;
        private readonly IProducerService<int, UpdateMarketFailedEvent> _producer;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<MarketUpdateCommandConsumer> _logger;
        private readonly IAsyncPolicy _retryPolicy;

        public MarketUpdateCommandConsumer(IConsumer<int, UpdateMarketCommand> consumer,
            IProducerService<int, UpdateMarketFailedEvent> producer, IMediator mediator, IMapper mapper,
            ILogger<MarketUpdateCommandConsumer> logger)
        {
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1),
                retryCount: 5);
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(delay,
                    (exception, span, retryCount, context) =>
                    {
                        _logger.LogError(exception,
                            $"Failed. Retry count: {retryCount}, exception message: {exception.Message}");
                    });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                _consumer.Subscribe(nameof(UpdateMarketCommand));
                await RunConsumerAsync(stoppingToken);
            }, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer?.Close();
            _consumer?.Dispose();
            await base.StopAsync(cancellationToken);
        }

        private async Task RunConsumerAsync(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                UpdateMarketCommand updateMarketCommand = null;
                try
                {
                    var message = _consumer.Consume(token);
                    updateMarketCommand = message.Message.Value;
                    _logger.LogInformation($"Consumer: {nameof(MarketUpdateCommandConsumer)}," +
                                           $"MessageKey: {message.Message.Key}, " +
                                           $"Topic: {message.Topic}, " +
                                           $"Partition: {message.Partition.Value}, " +
                                           $"Offset: {message.TopicPartitionOffset.Offset.Value}");

                    await _retryPolicy.ExecuteAsync(async x => await _mediator.Send(message.Message.Value, token),
                        token);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    _logger.LogError($"Exhausted all retries for message with Correlation: {updateMarketCommand?.CorrelationId}");
                    var updateMarketFailedEvent = _mapper.Map<UpdateMarketFailedEvent>(updateMarketCommand);
                    await _producer.ProduceAsync(updateMarketFailedEvent.MarketId, updateMarketFailedEvent);
                }
            }
        }
    }
}