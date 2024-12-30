using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee_Api.Models
{
    public class CakeVM
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("CakeName")]
        public required string CakeName { get; set; }
        [BsonElement("CakeDescription")]
        public required string CakeDescription { get; set; }
        [BsonElement("CakeImage")]
        public required string CakeImage { get; set; }
        [BsonElement("CakePrice")]
        public required decimal CakePrice { get; set; }
        [BsonElement("CakeRating")]
        public decimal CakeRating { get; set; } = 0;
    }
}
