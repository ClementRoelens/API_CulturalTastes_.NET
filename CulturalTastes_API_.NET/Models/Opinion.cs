using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CulturalTastes_API_.NET.Models
{
    public class Opinion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }
        public string content { get; set; }
        public int likes { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public string itemType { get; set; }
        public int? __v { get; set; }

        public Opinion(string content, string itemType, string authorId, string authorName)
        {
            this._id = ObjectId.GenerateNewId();
            this.content = content;
            this.likes = 0;
            this.authorId = authorId;
            this.authorName = authorName;
            this.itemType = itemType;
        }
        public Opinion(string content, string itemType, int likes, string authorId, string authorName)
        {
            this._id = ObjectId.GenerateNewId();
            this.content = content;
            this.likes = likes;
            this.authorId = authorId;
            this.authorName = authorName;
            this.itemType = itemType;
        }
    }
}
