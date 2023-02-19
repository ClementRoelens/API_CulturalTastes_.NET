using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CulturalTastes_API_.NET.Models
{
    public class Album
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? _id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public DateOnly date { get; set; }
        public int likes { get; set; }
        public int dislikes { get; set; }
        public List<string> opinionsId { get; set; }
        public string[] genres { get; set; }
        public string imageUrl { get; set; }
        public List<string> tags { get; set; }
        public int? __v { get; set; }
    }
}
