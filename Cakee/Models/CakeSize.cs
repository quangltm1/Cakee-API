using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cakee.Models
{
    public class CakeSize
    {
        [BsonId]
        [BsonRequired]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("SizeName")]
        public string? SizeName { get; set; }

    }
}
