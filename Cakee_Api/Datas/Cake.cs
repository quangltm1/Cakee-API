using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cakee_Api.Datas
{
    public class Cake
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("CakeName")]
        public required string CakeName { get; set; }
        [BsonElement("CakeDescription")]
        public required string CakeDescription { get; set; }
        [BsonElement("CakeStock")]
        public int CakeStock { get; set; } = 0;
        [BsonElement("CakePrice")]
        public decimal CakePrice { get; set; }
        [BsonElement("CakeImage")]
        public required string CakeImage { get; set; }
        [BsonElement("CakeRating")]
        public decimal CakeRating { get; set; } = 0;

        // Foreign Key
        [BsonElement("CakeCategoryId")]
        public ObjectId CakeCategoryId { get; set; }
        [BsonElement("CakeCategoryName")]
        public string CakeCategoryName { get; set; }
        [BsonElement("CakeSizeId")]
        public ObjectId CakeSizeId { get; set; }
        [BsonElement("CakeSizePrice")]
        public ObjectId CakeSizePrice { get; set; }

        public required ICollection<BillDetails> BillDetails { get; set; }
        public Cake()
        {
            BillDetails = new List<BillDetails>();
        }

    }
}
