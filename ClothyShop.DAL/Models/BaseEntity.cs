using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ClothyShop.DAL.Models
{
    public class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
    }
}
