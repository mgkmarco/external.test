using Bogus;
using Confluent.Kafka;
using External.Test.Contracts.Commands;
using External.Test.Contracts.Services;
using External.Test.Producers;
using External.Test.Tests.Unit.Fakers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace External.Test.Tests.Unit.ProducerServiceTests
{
    [Trait("Category", "Unit")]
    public class MarketUpdateFailedEventProducerTests
    {
        private readonly Mock<IProducer<int, UpdateMarketFailedEvent>> _producerMock;
        private readonly Mock<ILogger<MarketUpdateFailedEventProducer>> _loggerMock;
        private readonly IProducerService<int, UpdateMarketFailedEvent> _sut;

        public MarketUpdateFailedEventProducerTests()
        {
            _producerMock = new Mock<IProducer<int, UpdateMarketFailedEvent>>();
            _loggerMock = new Mock<ILogger<MarketUpdateFailedEventProducer>>();
            _sut = new MarketUpdateFailedEventProducer(_producerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ProduceAsync_Success_ShouldNotThrow_ProducesOneMessageOnly()
        {
            //Arrange
            var selectionCount = new Faker().Random.Int(1, 5);
            var updateMarketFailedEvent = FakerHelpers.FakeUpdateMarketFailedEvent(Guid.NewGuid(), selectionCount);
            var key = updateMarketFailedEvent.MarketId;
            var kafkaMessage = new Message<int, UpdateMarketFailedEvent>
            {
                Headers = new Headers {new Header("CorrelationId", updateMarketFailedEvent.CorrelationId.ToByteArray())},
                Key = key,
                Timestamp = new Timestamp(DateTimeOffset.UtcNow),
                Value = updateMarketFailedEvent
            };
            var deliveryResult = new DeliveryResult<int, UpdateMarketFailedEvent>();
            _producerMock.Setup(x => x.ProduceAsync("the-topic", kafkaMessage, CancellationToken.None))
                .ReturnsAsync(deliveryResult);

            // Act
            await _sut.ProduceAsync(key, updateMarketFailedEvent);

            // Assert
            _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            _producerMock.Verify(
                x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<int, UpdateMarketFailedEvent>>(),
                    CancellationToken.None), Times.Once);
        }
        
        [Fact]
        public async Task ProduceAsync_Failed_ShouldThrow_ProducesNoMessage()
        {
            //Arrange
            var selectionCount = new Faker().Random.Int(1, 5);
            var updateMarketFailedEvent = FakerHelpers.FakeUpdateMarketFailedEvent(Guid.NewGuid(), selectionCount);
            var key = updateMarketFailedEvent.MarketId;

            // Act
            await Assert.ThrowsAsync<NullReferenceException>(() => _sut.ProduceAsync(key, null));

            // Assert
            _loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            _producerMock.Verify(
                x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<int, UpdateMarketFailedEvent>>(),
                    CancellationToken.None), Times.Never);
        }
    }
}