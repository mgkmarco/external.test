using External.Test.Contracts.Enums;
using MediatR;
using System.Collections.Generic;

namespace External.Test.Contracts.Models
{
    public class MarketUpdate : IRequest
    {
        public int MatchId { get; set; }
        public int MarketId { get; set; }
        public string MarketType { get; set; }
        public MarketState MarketState { get; set; }
        public IEnumerable<MarketUpdateSelection> Selections { get; set; }
    }
}