using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace External.Test.Data.Contracts.Entities
{
    public class BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } 
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}