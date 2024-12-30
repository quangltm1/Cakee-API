using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee_Api.Datas
{
    public enum BillStatus
    {
        Pending = 0,
        Processing = 1,
        Delivering = 2,
        Done = 3,
        Cancel = 4
    }
    public class Bill
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("BillDeliveryDate")]
        public required DateTime BillDeliveryDate { get; set; }
        [BsonElement("BillReceiveDate")]
        public DateTime BillReceiveDate { get; set; }
        [BsonElement("BillDeliveryAddress")]
        public required string BillDeliveryAddress { get; set; }
        [BsonElement("BillStatus")]
        public required BillStatus BillStatus { get; set; }
        
        [BsonElement("BillDeposit")]
        public required decimal BillDeposit { get; set; }
        

        [BsonElement("BillTotal")]
        public required decimal BillTotal { get; set; }
        

        //relationship with client
        [BsonElement("BillClientId")]
        public required User BillClientId { get; set; }
        //relationship with cake
        [BsonElement("BillCakeId")]
        public required Cake BillCakeId { get; set; }
        //relationship with accessory
        [BsonElement("BillAcessoryId")]
        public required Acessory BillAcessoryId { get; set; }
        //relationship with billdetails
        [BsonElement("BillDetailsId")]
        public required BillDetails BillDetailsId { get; set; }

        public required ICollection<BillDetails> BillDetails { get; set; }
        public Bill()
        {
            BillDetails = new List<BillDetails>();
        }
    }
}
