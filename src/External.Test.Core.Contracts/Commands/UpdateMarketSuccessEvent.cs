using External.Test.Contracts.Enums;
using System;
using System.Collections.Generic;

namespace External.Test.Contracts.Commands
{
    public class UpdateMarketSuccessEvent
    {
        public bool Processed { get; } = true;
        public Guid CorrelationId { get; set; }
        public int MatchId { get; set; }
        public int MarketId { get; set; }
        public string MarketType { get; set; }
        public MarketState MarketState { get; set; }
        public IEnumerable<MarketUpdatedSelectionEvent> Selections { get; set; }
    }
}