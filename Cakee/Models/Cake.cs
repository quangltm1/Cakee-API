using Cakee.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
[BsonIgnoreExtraElements]
public class Cake
{
    [BsonId]
    [BsonRequired]
    public ObjectId Id { get; set; }

    [BsonRequired]
    [BsonElement("CakeName")]
    public required string CakeName { get; set; }

    [BsonElement("CakeSize")]
    public int CakeSize { get; set; }

    [BsonElement("CakeDescription")]
    public required string CakeDescription { get; set; }

    [BsonRequired]
    [BsonElement("CakePrice")]
    public required decimal CakePrice { get; set; }

    [BsonElement("CakeImage")]
    public string? CakeImage { get; set; }

    [BsonRequired]
    [BsonElement("CakeCategoryId")]
    public ObjectId CakeCategoryId { get; set; }

    [BsonElement("CakeRating")]
    public double CakeRating { get; set; }

    [BsonRequired]
    [BsonElement("CakeQuantity")]
    public int CakeQuantity { get; set; } =0;

    [BsonRequired]
    [BsonElement("UserId")]
    public ObjectId UserId { get; set; }

}

public class UpdateCakeRequest
{
    public string? CakeName { get; set; }
    public decimal? CakePrice { get; set; }
    public int? CakeSize { get; set; }
    public string? CakeCategoryId { get; set; }  // Nhận dạng string để dễ xử lý
    public string? CakeImage { get; set; }
    public int? CakeQuantity { get; set; }
}

