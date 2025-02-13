using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee.Models
{
    public class Acessory
    {
        [BsonId]
        [BsonRequired]
        public ObjectId Id { get; set; }
        [BsonRequired]
        [BsonElement("AcessoryName")]
        public string? AcessoryName { get; set; }
        [BsonRequired]
        [BsonElement("AcessoryPrice")]
        public decimal AcessoryPrice { get; set; }
    }
}
