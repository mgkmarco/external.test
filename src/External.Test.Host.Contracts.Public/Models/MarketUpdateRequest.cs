using External.Test.Host.Contracts.Public.Enums;
using System.Collections.Generic;

namespace External.Test.Host.Contracts.Public.Models
{
    /// <summary>
    /// Market Update Request data transfer object.  
    /// </summary>
    public class MarketUpdateRequest
    {
        /// <summary>
        /// The market identifier.
        /// <example>123</example> 
        /// </summary>
        public int MarketId { get; set; }
        /// <summary>
        /// The Market type.
        /// <example>Match Result</example>
        /// </summary>
        public string MarketType { get; set; }
        /// <summary>
        /// Market state. Must be a valid value of <see cref="Enums.MarketState"/>
        /// <example>Open</example> 
        /// </summary>
        public MarketState MarketState { get; set; }
        /// <summary>
        /// Market selections. />
        /// </summary>
        public IEnumerable<MarketSelectionUpdateRequest> Selections { get; set; }
    }
}