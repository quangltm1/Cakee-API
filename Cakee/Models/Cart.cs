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
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("UserId")]
        public String? UserId { get; set; }

        public List<CartItem> Items { get; set; } = new();

        [BsonRequired]
        [BsonElement("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }
}
