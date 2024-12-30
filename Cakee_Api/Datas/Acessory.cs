using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee_Api.Datas
{
    public class Acessory
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("AcessoryName")]
        public required string AcessoryName { get; set; }
        [BsonElement("AcessoryPrice")]
        public required decimal AcessoryPrice { get; set; }
    }
}
