using AutoMapper;
using External.Test.Contracts.Commands;
using External.Test.Data.Contracts.Entities;
using External.Test.Data.Contracts.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace External.Test.Handlers
{
    public class MarketUpdateHandler : IRequestHandler<UpdateMarketCommand, UpdateMarketResponse>
    {
        private readonly IRepository<MarketUpdateEntity> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<MarketUpdateHandler> _logger;

        public MarketUpdateHandler(IRepository<MarketUpdateEntity> repository, IMapper mapper, ILogger<MarketUpdateHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<UpdateMarketResponse> Handle(UpdateMarketCommand request, CancellationToken cancellationToken)
        {
            var marketUpdateEntity = _mapper.Map<MarketUpdateEntity>(request); 
            var filter = Builders<MarketUpdateEntity>.Filter.Eq(nameof(marketUpdateEntity.Id), marketUpdateEntity.Id);

            await _repository.GetCollectionContext()
                .ReplaceOneAsync(filter, marketUpdateEntity, new ReplaceOptions {IsUpsert = true}, cancellationToken);
            _logger.LogInformation($"Successfully inserted market update for CorrelationId: {marketUpdateEntity.Id}");
            
            return new UpdateMarketResponse(true);
        }
    }
}