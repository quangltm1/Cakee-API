using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cakee.Models
{
    public class Order
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("TotalPrice")]
        public decimal TotalPrice { get; set; }

        [BsonElement("OrderStatus")]
        public string OrderStatus { get; set; } // pending, completed, canceled

        [BsonElement("DeliveryStatus")]
        public string DeliveryStatus { get; set; } // pending, in_progress, delivered, failed

        [BsonElement("DeliveryDate")]
        public DateTime? DeliveryDate { get; set; }

        [BsonElement("DeliveryAddress")]
        public string DeliveryAddress { get; set; }

        [BsonElement("OrderDetails")]
        public List<OrderDetail> OrderDetails { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class OrderDetail
    {
        [BsonElement("ProductId")]
        public string ProductId { get; set; }

        [BsonElement("Quantity")]
        public int Quantity { get; set; }

        [BsonElement("Price")]
        public decimal Price { get; set; }
    }
}
