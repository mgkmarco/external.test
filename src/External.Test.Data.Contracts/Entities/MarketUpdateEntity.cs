using System.Collections.Generic;

namespace External.Test.Data.Contracts.Entities
{
    public class MarketUpdateEntity : BaseEntity
    {
        public int MatchId { get; set; }
        public int MarketId { get; set; }
        public string MarketType { get; set; }
        public int MarketState { get; set; }
        public IEnumerable<MarketSelectionEntity> Selections { get; set; }
    }
}