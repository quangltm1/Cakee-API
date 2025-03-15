using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cakee.Models
{
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // Chuyển đổi ObjectId thành string
        public string? Id { get; set; }

        [BsonElement("UserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }

}
