using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee.Models
{
    public enum BillStatus
    {
        Pending = 1,
        Processing = 2 ,
        Delivering = 3,
        Done = 0
    }
    public class Bill
    {
        [BsonId]
        [BsonRequired]
        public ObjectId Id { get; set; }
        [BsonRequired]
        [BsonElement("BillFullName")]
        public string BillFullName { get; set; }
        [BsonRequired]
        [BsonElement("BillName")]
        public string BillName { get; set; }
        [BsonRequired]
        [BsonElement("BillPhone")]
        public string BillPhone { get; set; }
        [BsonRequired]
        [BsonElement("BillAddress")]
        public string BillAddress { get; set; }
        [BsonRequired]
        [BsonElement("BillDeliveryDate")]
        public string BillDeliveryDate { get; set; }
        [BsonRequired]
        [BsonElement("BillReceiveDate")]
        public string BillReceiveDate { get; set; }
        [BsonRequired]
        [BsonElement("BillDeposit")]
        public string BillDeposit { get; set; }
        [BsonRequired]
        [BsonElement("BillTotal")]
        public string BillTotal { get; set; }
        [BsonRequired]
        [BsonElement("BillStatus")]
        public BillStatus BillStatus { get; set; } = BillStatus.Pending;
        [BsonRequired]
        [BsonElement("BillNote")]
        public string BillNote { get; set; }
        [BsonRequired]
        [BsonElement("BillContent")]
        public string BillContent { get; set; }
        [BsonId]
        [BsonRequired]
        public ObjectId UserId { get; set; }
        [BsonId]
        [BsonRequired]
        public List<ObjectId> CakeIds { get; set; }

    }
}
