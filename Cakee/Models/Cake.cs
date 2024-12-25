using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Cake
{
    [BsonId]
    [BsonRequired]
    public ObjectId Id { get; set; }  // Assuming MongoDB ObjectId is used
    [BsonRequired]
    [BsonElement("cakeName")]
    public string CakeName { get; set; }  // Correct property name
    public int CakeSize { get; set; }
    [BsonRequired]
    public decimal CakePrice { get; set; }
    public decimal CakeDiscount { get; set; }
    public string CakeImage { get; set; }
    [BsonRequired]
    public ObjectId CakeCategoryId { get; set; }
    public double CakeRating { get; set; }
    [BsonRequired]
    public int CakeQuantity { get; set; }

}
