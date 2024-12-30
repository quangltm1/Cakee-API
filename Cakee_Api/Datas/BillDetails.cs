using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee_Api.Datas
{
    public class BillDetails
    {
        [BsonElement("CakeId")]
        public ObjectId CakeId { get; set; }
        [BsonElement("BillId")]
        public ObjectId BillId { get; set; }
        [BsonElement("BillCakeQuantity")]
        public required int BillCakeQuantity { get; set; }
        [BsonElement("BillCakePrice")]
        public required decimal BillCakePrice { get; set; }
        [BsonElement("BillCakeTotal")]
        public required decimal BillCakeTotal { get; set; }
        [BsonElement("BillNote")]
        public required string BillNote { get; set; }
        [BsonElement("BillCakeContent")]
        public required string BillProductContent { get; set; }

        // Foreign Key
        [BsonElement("CakeSizeId")]
        public ObjectId CakeSizeId { get; set; }
        [BsonElement("AcessoryId")]
        public ObjectId AcessoryId { get; set; }
        public required Bill Bill { get; set; }
        public required Cake Cake { get; set; }

    }
}
