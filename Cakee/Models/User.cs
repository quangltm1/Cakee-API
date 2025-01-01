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
        [BsonElement("UserName")]
        public string UserName { get; set; }
        [BsonRequired]
        [BsonElement("PassWord")]
        public string PassWord { get; set; }
        [BsonRequired]
        [BsonElement("FullName")]
        public string FullName { get; set; }
        [BsonRequired]
        [BsonElement("Phone")]
        public string Phone { get; set; }

        [BsonRequired]
        [BsonElement("Role")]
        public int Role { get; set; }
        [BsonRequired]
        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class UserRegisterRequest
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public string FullName { get; set; }
        [Required]
        [MinLength(10)]
        public string Phone { get; set; }
    }

    public class UserLoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
