using AutoMapper;
using Bogus;
using External.Test.Contracts.Commands;
using External.Test.Data.Contracts.Entities;
using External.Test.Data.Contracts.Repositories;
using External.Test.Handlers;
using External.Test.Tests.Unit.Fakers;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace External.Test.Tests.Unit.Handlers
{
    [Trait("Category", "Unit")]
    public class MarketUpdateHandlerTests
    {
        private readonly Mock<IRepository<MarketUpdateEntity>> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<MarketUpdateHandler>> _loggerMock;
        private readonly IRequestHandler<UpdateMarketCommand, UpdateMarketResponse> _sut;

        public MarketUpdateHandlerTests()
        {
            _repositoryMock = new Mock<IRepository<MarketUpdateEntity>>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<MarketUpdateHandler>>();
            _sut = new MarketUpdateHandler(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Success_ShouldReturnUpdateMarketResponse_SuccessTrue()
        {
            //Arrange
            var selectionCount = new Faker().Random.Int(1, 5);
            var updateMarketCommand = FakerHelpers.FakeUpdateMarketCommand(selectionCount);
            var updateMarketEntity =
                FakerHelpers.FakeMarketUpdateEntity(updateMarketCommand.CorrelationId, selectionCount);
            var filter = Builders<MarketUpdateEntity>.Filter.Eq(nameof(updateMarketEntity.Id), updateMarketEntity.Id);
            var replaceOptions = new ReplaceOptions {IsUpsert = true};
            _mapperMock.Setup(x => x.Map<MarketUpdateEntity>(updateMarketCommand)).Returns(updateMarketEntity);
            _repositoryMock.Setup(x =>
                x.GetCollectionContext()
                    .ReplaceOneAsync(filter, updateMarketEntity, replaceOptions, CancellationToken.None));

            // Act
            var response = await _sut.Handle(updateMarketCommand, CancellationToken.None);

            // Assert
            Assert.True(response.Success);
            _repositoryMock.Verify(x => x.GetCollectionContext(), Times.Once);
        }
    }
}