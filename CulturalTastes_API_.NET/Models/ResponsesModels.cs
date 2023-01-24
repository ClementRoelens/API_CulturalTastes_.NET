namespace CulturalTastes_API_.NET.Models
{
    public class LikeOrDislikeResponse
    {
        public Film film { get; set; }
        public User user { get; set; }

        public LikeOrDislikeResponse(Film film, User user) {
            this.film = film;
            this.user = user;   
        }
    }

    public class CompleteResult
    {
        public Film film { get; set; }
        public User user { get; set; }
        public Opinion opinion { get; set; }
        public CompleteResult(Film film, User user, Opinion opinion)
        {
            this.film = film;
            this.user = user;
            this.opinion = opinion;
        }
    }
}
