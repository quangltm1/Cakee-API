using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cakee.Models
{
    public class Order
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; }

        [BsonElement("total_price")]
        public decimal TotalPrice { get; set; }

        [BsonElement("order_status")]
        public string OrderStatus { get; set; } // pending, completed, canceled

        [BsonElement("delivery_status")]
        public string DeliveryStatus { get; set; } // pending, in_progress, delivered, failed

        [BsonElement("delivery_date")]
        public DateTime? DeliveryDate { get; set; }

        [BsonElement("delivery_address")]
        public string DeliveryAddress { get; set; }

        [BsonElement("order_details")]
        public List<OrderDetail> OrderDetails { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class OrderDetail
    {
        [BsonElement("product_id")]
        public string ProductId { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }
    }
}
