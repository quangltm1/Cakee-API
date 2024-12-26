using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
[BsonIgnoreExtraElements]
public class Cake
{
    [BsonId]
    [BsonRequired]
    public ObjectId Id { get; set; }  // Assuming MongoDB ObjectId is used
    [BsonRequired]
    [BsonElement("cakeName")]
    public string CakeName { get; set; }  // Correct property name
    [BsonElement("cakeSize")]
    public int CakeSize { get; set; }
    [BsonElement("cakeDescription")]
    public string CakeDescription { get; set; }
    [BsonRequired]
    public decimal CakePrice { get; set; }
    public decimal CakeDiscount { get; set; }
    public string CakeImage { get; set; }
    [BsonRequired]
    public ObjectId CakeCategoryId { get; set; }
    public double CakeRating { get; set; }
    [BsonRequired]
    public int CakeQuantity { get; set; } = 0;

}
