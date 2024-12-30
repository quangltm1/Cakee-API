using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cakee.Models
{
    public class Rating
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; }

        [BsonElement("product_id")]
        public string ProductId { get; set; }

        [BsonElement("rating")]
        public int RatingValue { get; set; } // 1-5 stars

        [BsonElement("comment")]
        public string Comment { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
