using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee.Models
{
    public class CartItem
    {
        [BsonElement("ProductId")]
        public ObjectId ProductId { get; set; }


        public string ProductType { get; set; } // "Cake" hoặc "Accessory"
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
