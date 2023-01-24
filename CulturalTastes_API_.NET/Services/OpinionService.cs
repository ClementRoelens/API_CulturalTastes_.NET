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
            Opinion newOpinion = new Opinion(opinion.content, opinion.likes, opinion.author);
            await _opinionsCollection.InsertOneAsync(newOpinion);

            return await _opinionsCollection.Find(o => o._id == newOpinion._id).FirstAsync();
        }
    }
}
