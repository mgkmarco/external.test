using AutoMapper;
using External.Test.Contracts.Commands;
using External.Test.Contracts.Services;
using MediatR.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace External.Test.Behaviours
{
    public class MarketUpdatePostProcessor : IRequestPostProcessor<UpdateMarketCommand, UpdateMarketResponse>
    {
        private readonly IProducerService<int, UpdateMarketSuccessEvent> _producerService;
        private readonly IMapper _mapper;

        public MarketUpdatePostProcessor(IProducerService<int, UpdateMarketSuccessEvent> producerService,
            IMapper mapper)
        {
            _producerService = producerService ?? throw new ArgumentNullException(nameof(producerService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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