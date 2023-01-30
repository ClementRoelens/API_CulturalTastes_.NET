namespace CulturalTastes_API_.NET.Models
{
    public class CompleteResult
    {
        public Film item { get; set; }
        public User user { get; set; }
        public Opinion opinion { get; set; }
        public CompleteResult(Film film, User user, Opinion opinion)
        {
            this.item = film;
            this.user = user;
            this.opinion = opinion;
        }
    }
}
