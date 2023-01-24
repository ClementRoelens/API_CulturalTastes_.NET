namespace CulturalTastes_API_.NET.Models
{
    public class FilmsDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string FilmsCollectionName { get; set; } = null!;
    }
}
