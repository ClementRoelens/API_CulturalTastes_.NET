using Newtonsoft.Json;

namespace CulturalTastes_API_.NET.Models
{

    public class LikeItemResult
    {
        [JsonProperty]
        Film Film { get; set; }
        [JsonProperty]
        User User { get; set; }
        public LikeItemResult(Film film, User user)
        {
            this.Film = film;
            this.User = user;
        }
    }

    public class LikeOpinionResult
    {
        [JsonProperty]
        public Opinion Opinion { get; set; }
        [JsonProperty]
        public User User { get; set; }

        public LikeOpinionResult(Opinion opinion, User user)
        {
            this.Opinion = opinion;
            this.User = user;
        }
    }
    public class CompleteResult
    {
        [JsonProperty]
        public Film Film { get; set; }
        [JsonProperty]
        public User User { get; set; }
        [JsonProperty]
        public Opinion Opinion { get; set; }
        public CompleteResult(Film film, User user, Opinion opinion)
        {
            this.Film = film;
            this.User = user;
            this.Opinion = opinion;
        }
    }
}
