namespace CulturalTastes_API_.NET.Models
{
    public class AlbumsDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string AlbumsCollectionName { get; set; } = null!;
    }
}
