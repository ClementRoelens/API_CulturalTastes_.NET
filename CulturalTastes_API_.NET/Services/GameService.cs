using CulturalTastes_API_.NET.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CulturalTastes_API_.NET.Services
{
    public class GameService
    {
        private readonly IMongoCollection<Game> _gamesCollection;
        private readonly string[] _genres = {
            "Aventure",
            "Beat them all",
            "Course",
            "FPS",
            "Gestion",
            "Infiltration",
            "Plate-forme",
            "Réflexion",
            "RPG",
            "Shoot them up",
            "Simulation",
            "Stratégie",
            "Survival horror",
            "TPS"
        };
        public GameService(
          IOptions<GamesDatabaseSettings> gamesDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                gamesDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                gamesDatabaseSettings.Value.DatabaseName);

            _gamesCollection = mongoDatabase.GetCollection<Game>(
                gamesDatabaseSettings.Value.GamesCollectionName);

        }

        private const int _numberOfReturnedRandomGames = 20;
        private List<Game> GetNgames(List<Game> games)
        {
            Random rand = new Random();
            List<Game> transmittedGames = new List<Game>();
            int numberToReturn = (games.Count < _numberOfReturnedRandomGames) ? games.Count : _numberOfReturnedRandomGames;
            for (int i = 0; i < numberToReturn; i++)
            {
                int index = rand.Next(games.Count);
                transmittedGames.Add(games[index]);
                games.RemoveAt(index);
            }
            return transmittedGames;
        }

        public string[] GetGenres()
        {
            return _genres;
        }

        #region GetGames

        public async Task<List<Game>> GetGamesAsync(bool random)
        {
            List<Game> games = await _gamesCollection.Find(_ => true).ToListAsync();
            return random ? GetNgames(games) : games;
        }

        public async Task<List<Game>> GetGamesInOneAuthorAsync(string seekedAuthor, bool random)
        {
            var filter = Builders<Game>.Filter.Eq(game => game.author, seekedAuthor);
            List<Game> games = await _gamesCollection.Find(filter).ToListAsync();
            return random ? GetNgames(games) : games;
        }

        public async Task<List<Game>> GetGamesInOneGenreAsync(string seekedGenre, bool random)
        {
            var filter = Builders<Game>.Filter.AnyEq(game => game.genres, seekedGenre);
            List<Game> games = await _gamesCollection.Find(filter).ToListAsync();
            return random ? GetNgames(games) : games;
        }

        #endregion


        public async Task<Game?> GetOneGameAsync(string id) =>
            await _gamesCollection.Find(x => x._id == new ObjectId(id)).FirstOrDefaultAsync();

        public async Task<Game> GetOneRandomGameAsnc()
        {
            List<Game> games = await _gamesCollection.Find(_ => true).ToListAsync();
            Random rand = new Random();
            return games[rand.Next(games.Count)];
        }

        public async Task<List<Game>> Search(string searchedValue)
        {
            var filter = Builders<Game>.Filter.AnyEq("tags", searchedValue);
            List<Game> games = await _gamesCollection.Find(filter).ToListAsync();

            return (games.Count > _numberOfReturnedRandomGames) ? games.GetRange(0, _numberOfReturnedRandomGames) : games;

        }

        public async Task CreateGameAsync(Game newGame) =>
            await _gamesCollection.InsertOneAsync(newGame);

        public async Task<Game> UpdateGameLikesAsync(string id, int likesOperation, int dislikesOperation)
        {
            var filter = Builders<Game>.Filter.Eq(game => game._id, new ObjectId(id));
            var update = Builders<Game>.Update.Inc(game => game.likes, likesOperation).Inc(game => game.dislikes, dislikesOperation);
            var options = new FindOneAndUpdateOptions<Game> { ReturnDocument = ReturnDocument.After };
            return await _gamesCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Game> CreatedOpinionAsync(string gameId, string opinionId)
        {
            var filter = Builders<Game>.Filter.Eq(game => game._id, new ObjectId(gameId));
            var update = Builders<Game>.Update.Push(game => game.opinionsId, opinionId);
            var options = new FindOneAndUpdateOptions<Game> { ReturnDocument = ReturnDocument.After };
            return await _gamesCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Game> RemoveOpinionAsync(string gameId, List<string> newOpininsId)
        {
            var filter = Builders<Game>.Filter.Eq(game => game._id, new ObjectId(gameId));
            var update = Builders<Game>.Update.Set(game => game.opinionsId, newOpininsId);
            var options = new FindOneAndUpdateOptions<Game> { ReturnDocument = ReturnDocument.After };
            return await _gamesCollection.FindOneAndUpdateAsync(filter, update, options);
        }
    }
}
