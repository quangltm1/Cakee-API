using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cakee.Models
{
    public class Cart
    {
        [BsonId]
        [BsonRequired]
        public string Id { get; set; }

        [BsonRequired]
        [BsonElement("UserId")]
        public ObjectId UserId { get; set; }

        public List<CartItem> Items { get; set; } = new();

        [BsonRequired]
        [BsonElement("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }
}
