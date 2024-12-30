using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee_Api.Datas
{
    public class CakeSize
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("SizeName")]
        public required decimal SizeName { get; set; }
        [BsonElement("SizePrice")]
        public required decimal SizeValue { get; set; }
    }
}
