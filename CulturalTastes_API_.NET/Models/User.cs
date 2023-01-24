using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CulturalTastes_API_.NET.Models
{

    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }
        public string username { get; set; }
        public string? password { get; set; }
        public string? token { get; set; }
        public List<string> likedFilmsId { get; set; }
        public List<string> dislikedFilmsId { get; set; }
        public List<string> opinionsId { get; set; }
        public List<string> likedOpinionsId { get; set; }
        public bool isAdmin { get; set; }
        public int? __v { get; set; }

        public User(ObjectId id, string username, List<string> likedFilmsId, List<string> dislikedFilmsId,
           List<string> opinionsId, List<string> likedOpinionsId, bool isAdmin)
        {
            this._id = id;
            this.username = username;
            this.likedFilmsId = likedFilmsId;
            this.dislikedFilmsId = dislikedFilmsId;
            this.opinionsId = opinionsId;
            this.likedOpinionsId = likedOpinionsId;
            this.isAdmin = isAdmin;
        }

        public User(ObjectId id, string username, List<string> likedFilmsId, List<string> dislikedFilmsId,
            List<string> opinionsId, List<string> likedOpinionsId, bool isAdmin, string token)
        {
            this._id = id;
            this.username = username;
            this.likedFilmsId = likedFilmsId;
            this.dislikedFilmsId = dislikedFilmsId;
            this.opinionsId = opinionsId;
            this.likedOpinionsId = likedOpinionsId;
            this.isAdmin = isAdmin;
            this.token = token;
        }
    }
}
