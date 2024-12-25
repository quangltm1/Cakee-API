using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee.Models
{
    public class Bill
    {
        [BsonId]
        [BsonRequired]
        public ObjectId Id { get; set; }
        [BsonRequired]
        public string BillFullName { get; set; }
        [BsonRequired]
        public string BillName { get; set; }
        [BsonRequired]
        public string BillPhone { get; set; }
        [BsonRequired]
        public string BillAddress { get; set; }
        [BsonRequired]
        public string BillDeliveryDate { get; set; }
        [BsonRequired]
        public string BillReceiveDate { get; set; }
        [BsonRequired]
        public string BillDeposit { get; set; }
        [BsonRequired]
        public string BillTotal { get; set; }
        [BsonRequired]
        public string BillStatus { get; set; }
        [BsonRequired]
        public string BillNote { get; set; }
        [BsonRequired]
        public string BillContent { get; set; }
        [BsonId]
        [BsonRequired]
        public ObjectId UserId { get; set; }
        [BsonId]
        [BsonRequired]
        public List<ObjectId> CakeIds { get; set; }

    }
}
