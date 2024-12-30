using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee_Api.Datas
{
    public class Category
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("CategoryName")]
        public required string CategoryName { get; set; }
    }
}
