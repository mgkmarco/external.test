using AutoMapper;
using External.Test.Contracts.Commands;
using External.Test.Contracts.Services;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace External.Test.Behaviours
{
    public class MarketUpdatePostProcessor : IRequestPostProcessor<UpdateMarketCommand, UpdateMarketResponse>
    {
        private readonly IProducerService<int, UpdateMarketSuccessEvent> _producerService;
        private readonly IMapper _mapper;
        private readonly ILogger<MarketUpdatePostProcessor> _logger;

        public MarketUpdatePostProcessor(IProducerService<int, UpdateMarketSuccessEvent> producerService,
            IMapper mapper, ILogger<MarketUpdatePostProcessor> logger)
        {
            _producerService = producerService ?? throw new ArgumentNullException(nameof(producerService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Process(UpdateMarketCommand request, UpdateMarketResponse response,
            CancellationToken cancellationToken)
        {
            if (response.Success)
            {
                var successEvent = _mapper.Map<UpdateMarketSuccessEvent>(request);
                await _producerService.ProduceAsync(successEvent.MarketId, successEvent);
            }
        }
    }
}