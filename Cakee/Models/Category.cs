using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cakee.Models
{
    public class Category
    {
        [BsonId]
        [BsonRequired]
        public ObjectId Id { get; set; }
        [BsonRequired]
        [BsonElement("CategoryName")]
        public string CategoryName { get; set; }
        

        
    }
}
