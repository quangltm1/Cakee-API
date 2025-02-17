using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee.Models
{
    public class CartItem
    {
        [BsonRequired]
        [BsonElement("CakeId")]
        public ObjectId CakeId { get; set; }

        [BsonElement("Acessory")]
        public ObjectId Acessory { get; set; }
        [BsonRequired]
        [BsonElement("QuantityCake")]
        public int QuantityCake { get; set; }
        [BsonElement("QuantityAccessory")]
        public int QuantityAccessory { get; set; }
        [BsonElement("Total")]
        public decimal Total { get; set; }
    }
}
