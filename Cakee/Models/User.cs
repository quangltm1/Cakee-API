using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cakee.Models
{
    public class User
    {
        [BsonId]
        public Object Id { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        [Required]
        [StringLength(100)]
        public string Password { get; set; }
        [Required]
        [StringLength(50)]
        public string FullName { get; set; }
        [Required]
        [StringLength(11)]
        public string Phone { get; set; }
        [Required]
        [StringLength(1)]
        public int Role { get; set; }
    }
}
