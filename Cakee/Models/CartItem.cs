using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee.Models
{
    public class CartItem
    {
        [BsonRequired]
        [BsonElement("CakeId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public String? CakeId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("AcessoryId")]
        public String? AcessoryId { get; set; }

        [BsonRequired]
        [BsonElement("QuantityCake")]
        public int QuantityCake { get; set; }
        [BsonElement("QuantityAccessory")]
        public int QuantityAccessory { get; set; }
        [BsonElement("Total")]
        public decimal Total { get; set; }
    }
}
