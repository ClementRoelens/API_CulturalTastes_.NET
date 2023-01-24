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
        public string author { get; set; }
        public int? __v { get; set; }

        public Opinion(string content, string author)
        {
            this._id = ObjectId.GenerateNewId();
            this.content = content;
            this.likes = 0;
            this.author = author;
        }
        public Opinion(string content, int likes, string author)
        {
            this._id = ObjectId.GenerateNewId();
            this.content = content;
            this.likes = likes;
            this.author = author;
        }
    }
}
