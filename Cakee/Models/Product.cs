using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
[BsonIgnoreExtraElements]
public class Product
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("cakeName")]
    public string ProductName { get; set; }

    [BsonElement("cakeDescription")]
    public string Description { get; set; }
    [BsonElement("cakeSize")]
    public double Size { get; set; }

    [BsonElement("cakePrice")]
    public decimal Price { get; set; }

    [BsonElement("cakeCategoryId")]
    public string CategoryId { get; set; }

    [BsonElement("cakeImage")]
    public string ImageUrl { get; set; }

    [BsonElement("cakeQuantity")]
    public int Stock { get; set; }
    [BsonElement("cakeRating")]
    public double Rating { get; set; }

    [BsonElement("created_by")]
    public string CreatedBy { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }
}
