using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee_Api.Datas
{
    public enum Role
    {
        Admin = 1,
        User = 0
    }
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("UserFullName")]
        public required string UserFullName { get; set; }
        [BsonElement("UserName")]
        public required string UserName { get; set; }
        [BsonElement("UserPassword")]
        public required string UserPassword { get; set; }
        [BsonElement("UserPhone")]
        public required string UserPhone { get; set; }
        [BsonElement("UserAddress")]
        public required string UserAddress { get; set; }
        [BsonElement("UserRole")]
        public required Role UserRole { get; set; }

    }
}
