using CulturalTastes_API_.NET.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;

namespace CulturalTastes_API_.NET.Services
{
    public class OpinionService
    {
        private readonly IMongoCollection<Opinion> _opinionsCollection;
        public OpinionService(
            IOptions<OpinionsDatabaseSettings> opinionsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                opinionsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                opinionsDatabaseSettings.Value.DatabaseName);

            _opinionsCollection = mongoDatabase.GetCollection<Opinion>(
                opinionsDatabaseSettings.Value.OpinionsCollectionName);

        }

        public async Task<Opinion> GetOneOpinionAsync(string id)
        {
            return await _opinionsCollection.Find(opinion => opinion._id == new ObjectId(id)).FirstOrDefaultAsync();
        }

        public async Task<Opinion> CreateOpinionAsync(Opinion opinion)
        {
            Opinion newOpinion = new Opinion(opinion.content,opinion.itemType,opinion.authorId,opinion.authorName);
            await _opinionsCollection.InsertOneAsync(newOpinion);
            return await _opinionsCollection.Find(opinion => opinion._id == newOpinion._id).FirstAsync();
        }

        public async Task<Opinion> LikeOrDislikeOpinionAsync(string opinionId, int operation)
        {
            var filter = Builders<Opinion>.Filter.Eq(opinion => opinion._id, new ObjectId(opinionId));
            var update = Builders<Opinion>.Update.Inc(opinion => opinion.likes, operation);
            var options = new FindOneAndUpdateOptions<Opinion> { ReturnDocument = MongoDB.Driver.ReturnDocument.After };
            return await _opinionsCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<Opinion> ModifyOpinion(string id, string content)
        {
            var filter = Builders<Opinion>.Filter.Eq(opinion => opinion._id, new ObjectId(id));
            var update = Builders<Opinion>.Update.Set(opinion => opinion.content, content);
            var options = new FindOneAndUpdateOptions<Opinion> { ReturnDocument = MongoDB.Driver.ReturnDocument.After };
            return await _opinionsCollection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task RemoveOpinionAsync(string opinionId)
        {
            await _opinionsCollection.DeleteOneAsync(opinion => opinion._id == new ObjectId(opinionId));
        }
    }
}
