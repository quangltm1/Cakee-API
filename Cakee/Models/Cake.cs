using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Cake
{
    [BsonId]
    public ObjectId Id { get; set; }  // Assuming MongoDB ObjectId is used
    [BsonElement("cakeName")]
    public string CakeName { get; set; }  // Correct property name
    public string CakeDescription { get; set; }
    public decimal CakePrice { get; set; }
    public decimal CakeDiscount { get; set; }
    public string CakeImage { get; set; }
    public string CakeCategoryId { get; set; }
    public double CakeRating { get; set; }
}
