﻿using Cakee.Models;
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

}
