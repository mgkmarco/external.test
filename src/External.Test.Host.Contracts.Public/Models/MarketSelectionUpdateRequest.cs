namespace External.Test.Host.Contracts.Public.Models
{
    /// <summary>
    /// Selection Request data transfer object.  
    /// </summary>
    public class MarketSelectionUpdateRequest
    {
        /// <summary>
        /// The name of the selection.
        /// <example>Home</example>
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Price
        /// <example>1.65</example>
        /// </summary>
        public double Price { get; set; }
    }
}