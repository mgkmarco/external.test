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
    public class MarketUpdateSuccessEventProducerTest
    {
        private readonly Mock<IProducer<int, UpdateMarketSuccessEvent>> _producerMock;
        private readonly Mock<ILogger<MarketUpdateSuccessEventProducer>> _loggerMock;
        private readonly IProducerService<int, UpdateMarketSuccessEvent> _sut;

        public MarketUpdateSuccessEventProducerTest()
        {
            _producerMock = new Mock<IProducer<int, UpdateMarketSuccessEvent>>();
            _loggerMock = new Mock<ILogger<MarketUpdateSuccessEventProducer>>();
            _sut = new MarketUpdateSuccessEventProducer(_producerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ProduceAsync_Success_ShouldNotThrow_ProducesOneMessageOnly()
        {
            //Arrange
            var selectionCount = new Faker().Random.Int(1, 5);
            var updateMarketSuccessEvent = FakerHelpers.FakeUpdateMarketSuccessEvent(Guid.NewGuid(), selectionCount);
            var key = updateMarketSuccessEvent.MarketId;
            var kafkaMessage = new Message<int, UpdateMarketSuccessEvent>
            {
                Headers = new Headers {new Header("CorrelationId", updateMarketSuccessEvent.CorrelationId.ToByteArray())},
                Key = key,
                Timestamp = new Timestamp(DateTimeOffset.UtcNow),
                Value = updateMarketSuccessEvent
            };
            var deliveryResult = new DeliveryResult<int, UpdateMarketSuccessEvent>();
            _producerMock.Setup(x => x.ProduceAsync("the-topic", kafkaMessage, CancellationToken.None))
                .ReturnsAsync(deliveryResult);

            // Act
            await _sut.ProduceAsync(key, updateMarketSuccessEvent);

            // Assert
            _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            _producerMock.Verify(
                x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<int, UpdateMarketSuccessEvent>>(),
                    CancellationToken.None), Times.Once);
        }
        
        [Fact]
        public async Task ProduceAsync_Failed_ShouldThrow_ProducesNoMessage()
        {
            //Arrange
            var selectionCount = new Faker().Random.Int(1, 5);
            var updateMarketSuccessEvent = FakerHelpers.FakeUpdateMarketSuccessEvent(Guid.NewGuid(), selectionCount);
            var key = updateMarketSuccessEvent.MarketId;

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
                x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<int, UpdateMarketSuccessEvent>>(),
                    CancellationToken.None), Times.Never);
        }
    }
}