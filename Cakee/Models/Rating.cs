using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cakee.Models
{
    public class Rating
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("ProductId")]
        public string ProductId { get; set; }

        [BsonElement("RatingValue")]
        public int RatingValue { get; set; } // 1-5 stars

        [BsonElement("Comment")]
        public string Comment { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }
}
