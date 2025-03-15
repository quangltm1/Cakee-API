using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace Cakee.Models
{
    public class CustomDateTimeSerializer : SerializerBase<DateTime>
    {
        public override DateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.GetCurrentBsonType();
            if (bsonType == BsonType.String)
            {
                var dateAsString = context.Reader.ReadString();
                if (DateTime.TryParse(dateAsString, out var date))
                {
                    return date;
                }
                throw new FormatException($"String '{dateAsString}' was not recognized as a valid DateTime.");
            }
            return base.Deserialize(context, args);
        }
    }
    public enum BillStatus
    {
        Pending = 1,
        Processing = 2 ,
        Delivering = 3,
        Done = 0
    }
    public class Bill
    {
        [BsonId]
        [BsonRequired]
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("BillCakeSizeId")]
        public String? BillCakeSizeId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("BillAcessoriesId")]
        public String? BillAcessoriesId { get; set; }

        [BsonRequired]
        [BsonElement("BillDeliveryCustomName")]
        public string? BillDeliveryCustomName { get; set; }


        [BsonRequired]
        [BsonElement("BillDeliveryAddress")]
        public string? BillDeliveryAddress { get; set; }

        [BsonRequired]
        [BsonElement("BillDeliveryPhone")]
        public string? BillDeliveryPhone { get; set; }

        [BsonRequired]
        [BsonElement("BillDeliveryDate")]
        public DateTime BillDeliveryDate { get; set; }

        [BsonRequired]
        [BsonElement("BillDeposit")]
        public Decimal BillDeposit { get; set; }

        [BsonRequired]
        [BsonElement("BillNote")]
        public string? BillNote { get; set; }

        [BsonRequired]
        [BsonElement("BillReceiveDate")]
        public DateTime BillReceiveDate { get; set; }

        [BsonRequired]
        [BsonElement("BillStatus")]
        public BillStatus BillStatus { get; set; } = BillStatus.Pending;

        [BsonRequired]
        [BsonElement("BillTotal")]
        public Decimal BillTotal { get; set; }

        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("BillCustomId")]
        public String? BillCustomId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("BillShopId")]
        public String? BillShopId { get; set; }

        [BsonRequired]
        [BsonElement("BillCakeContent")]
        public string? BillCakeContent { get; set; }

        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("BillCakeId")]
        public String? BillCakeId { get; set; }

        [BsonRequired]
        [BsonElement("BillCakeQuantity")]
        public int BillCakeQuantity { get; set; }

    }
}
