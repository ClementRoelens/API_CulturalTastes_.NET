namespace CulturalTastes_API_.NET.Models
{
    public class OpinionsDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string OpinionsCollectionName { get; set; } = null!;
    }
}
