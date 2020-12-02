using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace External.Test.Data.Contracts.Entities
{
    public class MarketSelectionEntity
    {
        public string Name { get; set; }
        [BsonRepresentation(BsonType.Double)]
        public double Price { get; set; }
    }
}