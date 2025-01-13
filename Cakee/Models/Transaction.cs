using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee.Models
{
    public enum TransactionStatus
    {
        Pending = 1,
        Processing = 2,
        Delivering = 3,
        Done = 0
    }
    public class Transaction
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("TransactionStatus")]
        public TransactionStatus TransactionStatus { get; set; }
        [BsonElement("TransactionUserId")]
        public string? TransactionUserId { get; set; }
        [BsonElement("TransactionFullName")]
        public string? TransactionFullName { get; set; }
        [BsonElement("TransactionPhone")]
        public string? TransactionPhone { get; set; }
        [BsonElement("TransactionAmount")]
        public decimal TransactionAmount { get; set; }
        [BsonElement("TransactionPayment")]
        public string? TransactionPayment { get; set; }
        [BsonElement("TransactionCreateAt")]
        public DateTime TransactionCreateAt { get; set; }

    }
}
