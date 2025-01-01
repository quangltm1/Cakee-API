using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee.Models
{
    public class Accessory
    {
        [BsonId]
        [BsonRequired]
        public ObjectId Id { get; set; }
        [BsonRequired]
        [BsonElement("AccessoryName")]
        public string AccessoryName { get; set; }
        [BsonRequired]
        [BsonElement("AccessoryPrice")]
        public decimal AccessoryPrice { get; set; }
    }
}
