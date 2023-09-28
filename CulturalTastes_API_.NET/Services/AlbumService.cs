using CulturalTastes_API_.NET.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CulturalTastes_API_.NET.Services
{
    public class AlbumService
    {
        private readonly IMongoCollection<Album> _albumsCollection;
        private readonly string[] _genres = {
            "Blues",
            "Country",
            "Classique",
            "Electro",
            "Folk",
            "Funk",
            "Jazz",
            "Métal",
            "Pop",
            "Punk",
            "Rap",
            "Reggae",
            "Rock",
            "Soul"
        };
        public AlbumService(
          IOptions<AlbumsDatabaseSettings> albumsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                albumsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                albumsDatabaseSettings.Value.DatabaseName);

            _albumsCollection = mongoDatabase.GetCollection<Album>(
                albumsDatabaseSettings.Value.AlbumsCollectionName);

        }

        private const int _numberOfReturnedRandomAlbums = 20;
        private List<Album> GetNalbums(List<Album> albums)
        {
            Random rand = new Random();
            List<Album> transmittedAlbums = new List<Album>();
            int numberToReturn = (albums.Count < _numberOfReturnedRandomAlbums) ? albums.Count : _numberOfReturnedRandomAlbums;
            for (int i = 0; i < numberToReturn; i++)
            {
                int index = rand.Next(albums.Count);
                transmittedAlbums.Add(albums[index]);
                albums.RemoveAt(index);
            }
            return transmittedAlbums;
        }

        public string[] GetGenres()
        {
            return _genres;
        }

        #region GetAlbums

        public async Task<List<Album>> GetAlbumsAsync(bool random)
        {
            List<Album> albums = await _albumsCollection.Find(_ => true).ToListAsync();
            return random ? GetNalbums(albums) : albums;
        }

        public async Task<List<Album>> GetAlbumsInOneAuthorAsync(string seekedAuthor, bool random)
        {
            var filter = Builders<Album>.Filter.Eq(album => album.author, seekedAuthor);
            List<Album> albums = await _albumsCollection.Find(filter).ToListAsync();
            return random ? GetNalbums(albums) : albums;
        }

        public async Task<List<Album>> GetAlbumsInOneGenreAsync(string seekedGenre, bool random)
        {
            var filter = Builders<Album>.Filter.AnyEq(album => album.genres, seekedGenre);
            List<Album> albums = await _albumsCollection.Find(filter).ToListAsync();
            return random ? GetNalbums(albums) : albums;
        }

        #endregion


        public async Task<Album?> GetOneAlbumAsync(string id) =>
            await _albumsCollection.Find(x => x._id == new ObjectId(id)).FirstOrDefaultAsync();

        public async Task<Album> GetOneRandomAlbumAsnc()
        {
            List<Album> albums = await _albumsCollection.Find(_ => true).ToListAsync();
            Random rand = new Random();
            return albums[rand.Next(albums.Count)];
        }

        public async Task<List<Album>> Search(string searchedValue)
        {
            var filter = Builders<Album>.Filter.AnyEq("tags", searchedValue);
            List<Album> albums = await _albumsCollection.Find(filter).ToListAsync();

            return (albums.Count > _numberOfReturnedRandomAlbums) ? albums.GetRange(0, _numberOfReturnedRandomAlbums) : albums;

        }

        public async Task CreateAlbumAsync(Album newAlbum) =>
            await _albumsCollection.InsertOneAsync(newAlbum);

        public async Task<Album> UpdateAlbumLikesAsync(string id, int likesOperation, int dislikesOperation)
        {
            var filter = Builders<Album>.Filter.Eq(album => album._id, new ObjectId(id));
            var update = Builders<Album>.Update.Inc(album => album.likes, likesOperation).Inc(album => album.dislikes, dislikesOperation);
            var options = new FindOneAndUpdateOptions<Album> { ReturnDocument = ReturnDocument.After };
            return await _albumsCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Album> CreatedOpinionAsync(string albumId, string opinionId)
        {
            var filter = Builders<Album>.Filter.Eq(album => album._id, new ObjectId(albumId));
            var update = Builders<Album>.Update.Push(album => album.opinionsId, opinionId);
            var options = new FindOneAndUpdateOptions<Album> { ReturnDocument = ReturnDocument.After };
            return await _albumsCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Album> RemoveOpinionAsync(string albumId, List<string> newOpininsId)
        {
            var filter = Builders<Album>.Filter.Eq(album => album._id, new ObjectId(albumId));
            var update = Builders<Album>.Update.Set(album => album.opinionsId, newOpininsId);
            var options = new FindOneAndUpdateOptions<Album> { ReturnDocument = ReturnDocument.After };
            return await _albumsCollection.FindOneAndUpdateAsync(filter, update, options);
        }

    }
}
