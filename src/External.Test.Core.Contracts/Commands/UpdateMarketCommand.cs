using External.Test.Contracts.Enums;
using External.Test.Contracts.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace External.Test.Contracts.Commands
{
    public class UpdateMarketCommand : IRequest<UpdateMarketResponse>
    {
        public Guid CorrelationId { get; set; }
        public int MatchId { get; set; }
        public int MarketId { get; set; }
        public string MarketType { get; set; }
        public MarketState MarketState { get; set; }
        public IEnumerable<UpdateMarketSelectionCommand> Selections { get; set; }
    }
}