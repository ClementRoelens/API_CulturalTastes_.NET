using CulturalTastes_API_.NET.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CulturalTastes_API_.NET.Services
{
    public class FilmService
    {

        private readonly IMongoCollection<Film> _filmsCollection;
        private readonly string[] _genres = {
            "Action",
    "Aventure",
    "Comédie",
    "Drame",
    "Fantasy",
    "Guerre",
    "Historique",
    "Horreur",
    "Romance",
    "Science-fiction",
    "Thriller",
    "Western"
        };
        public FilmService(
            IOptions<FilmsDatabaseSettings> filmsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                filmsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                filmsDatabaseSettings.Value.DatabaseName);

            _filmsCollection = mongoDatabase.GetCollection<Film>(
                filmsDatabaseSettings.Value.FilmsCollectionName);

        }

        private const int _numberOfReturnedRandomFilms = 20;
        private List<Film> GetNfilms(List<Film> films)
        {
            Random rand = new Random();
            List<Film> transmittedFilms = new List<Film>();
            int numberToReturn = (films.Count < _numberOfReturnedRandomFilms) ? films.Count : _numberOfReturnedRandomFilms;
            for (int i = 0; i<numberToReturn; i++)
            {
                int index = rand.Next(films.Count);
                transmittedFilms.Add(films[index]);
                films.RemoveAt(index);
            }
            return transmittedFilms;
        }

        public string[] GetGenres()
        {
            return _genres;
        }

        #region GetFilms

        public async Task<List<Film>> GetFilmsAsync(bool random)
        {
            List<Film> films = await _filmsCollection.Find(_ => true).ToListAsync();
            return random ? GetNfilms(films) : films;
        }

        public async Task<List<Film>> GetFilmsInOneAuthorAsync(string seekedAuthor, bool random)
        {
            var filter = Builders<Film>.Filter.Eq(film => film.author, seekedAuthor);
            List<Film> films = await _filmsCollection.Find(filter).ToListAsync();
            return random ? GetNfilms(films) : films;
        }

        public async Task<List<Film>> GetFilmsInOneGenreAsync(string seekedGenre, bool random)
        {
            var filter = Builders<Film>.Filter.AnyEq(film => film.genres, seekedGenre);
            List<Film> films = await _filmsCollection.Find(filter).ToListAsync();
            return random ? GetNfilms(films) : films;
        }

        #endregion


        public async Task<Film?> GetOneFilmAsync(string id) =>
            await _filmsCollection.Find(x => x._id == new ObjectId(id)).FirstOrDefaultAsync();

        public async Task<Film> GetOneRandomFilmAsnc()
        {
            List<Film> films = await _filmsCollection.Find(_ => true).ToListAsync();
            Random rand = new Random();
            return films[rand.Next(films.Count)];
        }


        public async Task CreateFilmAsync(Film newFilm) =>
            await _filmsCollection.InsertOneAsync(newFilm);

        public async Task<Film> UpdateFilmLikesAsync(string id, int likesOperation, int dislikesOperation)
        {
            var filter = Builders<Film>.Filter.Eq(film=>film._id, new ObjectId(id));
            var update = Builders<Film>.Update.Inc(film => film.likes, likesOperation).Inc(film => film.dislikes, dislikesOperation);
            var options = new FindOneAndUpdateOptions<Film> { ReturnDocument = ReturnDocument.After };
            return await _filmsCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Film> CreatedOpinionAsync(string filmId, string opinionId)
        {
            var filter = Builders<Film>.Filter.Eq(film => film._id, new ObjectId(filmId));
            var update = Builders<Film>.Update.Push(film => film.opinionsId,opinionId);
            var options = new FindOneAndUpdateOptions<Film> { ReturnDocument = ReturnDocument.After };
            return await _filmsCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Film> RemoveOpinionAsync(string filmId, List<string> newOpininsId)
        {
            var filter = Builders<Film>.Filter.Eq(film => film._id, new ObjectId(filmId));
            var update = Builders<Film>.Update.Set(film => film.opinionsId, newOpininsId);
            var options = new FindOneAndUpdateOptions<Film> { ReturnDocument = ReturnDocument.After };
            return await _filmsCollection.FindOneAndUpdateAsync(filter, update, options);
        }

    }
}
