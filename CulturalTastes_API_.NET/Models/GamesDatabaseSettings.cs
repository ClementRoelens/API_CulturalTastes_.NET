using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CulturalTastes_API_.NET.Models
{
    public class GamesDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string GamesCollectionName { get; set; } = null!;
    }
}
