using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cakee.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonRequired]
        public string UserName { get; set; }
        [BsonRequired]
        public string PassWord { get; set; }
        [BsonRequired]
        public string FullName { get; set; }
        [BsonRequired]
        public string Phone { get; set; }

        [BsonRequired]
        public int Role { get; set; }
    }
}
