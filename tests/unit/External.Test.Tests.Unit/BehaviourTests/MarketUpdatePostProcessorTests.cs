using AutoMapper;
using Bogus;
using External.Test.Behaviours;
using External.Test.Contracts.Commands;
using External.Test.Contracts.Services;
using External.Test.Tests.Unit.Fakers;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace External.Test.Tests.Unit.BehaviourTests
{
    [Trait("Category", "Unit")]
    public class MarketUpdatePostProcessorTests
    {
        private readonly Mock<IProducerService<int, UpdateMarketSuccessEvent>> _producerServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly MarketUpdatePostProcessor _sut;

        public MarketUpdatePostProcessorTests()
        {
            _producerServiceMock = new Mock<IProducerService<int, UpdateMarketSuccessEvent>>();
            _mapperMock = new Mock<IMapper>();
            _sut = new MarketUpdatePostProcessor(_producerServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Process_Success_ShouldReturnResponseIsSuccessTrue_ShouldProduceEvent()
        {
            //Arrange
            var selectionCount = new Faker().Random.Int(1, 5);
            var updateMarketCommand = FakerHelpers.FakeUpdateMarketCommand(selectionCount);
            var updateMarketResponse = new UpdateMarketResponse(true);
            var updateMarketSuccessEvent = FakerHelpers.FakeUpdateMarketSuccessEvent(updateMarketCommand.CorrelationId, selectionCount);
            
            _mapperMock.Setup(x => x.Map<UpdateMarketSuccessEvent>(updateMarketCommand)).Returns(updateMarketSuccessEvent);
            _producerServiceMock.Setup(x => x.ProduceAsync(updateMarketSuccessEvent.MarketId, updateMarketSuccessEvent))
                .Returns(Task.CompletedTask);
            
            // Act
            await _sut.Process(updateMarketCommand, updateMarketResponse, CancellationToken.None);

            // Assert
            Assert.True(updateMarketResponse.Success);
            Assert.True(updateMarketCommand.CorrelationId == updateMarketSuccessEvent.CorrelationId);
            Assert.True(updateMarketCommand.Selections.Count() == updateMarketSuccessEvent.Selections.Count());
            Assert.True(updateMarketSuccessEvent.Processed);
            _producerServiceMock.Verify(x => x.ProduceAsync(It.IsAny<int>(), It.IsAny<UpdateMarketSuccessEvent>()), Times.Once);
        }
        
        [Fact]
        public async Task Process_Success_ShouldReturnResponseIsSuccessFalse_ShouldNotProduceEvent()
        {
            //Arrange
            var selectionCount = new Faker().Random.Int(1, 5);
            var updateMarketCommand = FakerHelpers.FakeUpdateMarketCommand(selectionCount);
            var updateMarketResponse = new UpdateMarketResponse(false);
            
            // Act
            await _sut.Process(updateMarketCommand, updateMarketResponse, CancellationToken.None);

            // Assert
            Assert.False(updateMarketResponse.Success);
            _producerServiceMock.Verify(x => x.ProduceAsync(It.IsAny<int>(), It.IsAny<UpdateMarketSuccessEvent>()), Times.Never);
        }
    }
}