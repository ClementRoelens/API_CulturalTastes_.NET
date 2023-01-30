using CulturalTastes_API_.NET.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace CulturalTastes_API_.NET.Services
{
    public class UserService
    {

        private readonly IMongoCollection<User> _usersCollection;
        private readonly IConfiguration _configuration;

        public UserService(
            IOptions<UsersDatabaseSettings> usersDatabaseSettings,
            IConfiguration configuration)
        {
            var mongoClient = new MongoClient(
                usersDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                usersDatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                usersDatabaseSettings.Value.UsersCollectionName);

            _configuration = configuration;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            Console.WriteLine("UserService.Login()");
            User user = await _usersCollection.Find(user => user.username == username).FirstOrDefaultAsync();
            Console.WriteLine($"Utilisateur trouvé : {user.username}");

            Console.WriteLine("Le password n'est pas nul");
            if (BC.Verify(password, user.password))
            {
                Console.WriteLine("Password correct");

                var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("userId", user._id.ToString())
                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: signIn);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                User transmittedUser = new User(
                    user._id,
                    user.username,
                    user.likedFilmsId,
                    user.dislikedFilmsId,
                    user.opinionsId,
                    user.likedOpinionsId,
                    user.isAdmin, tokenString);

                return transmittedUser;
            }
            else
            {
                Console.WriteLine("Password incorrect");
                return null;
            }
        }

        public async Task<User> Signup(string username, string password)
        {
            string hashedPassword = BC.HashString(password);
            User user = new User(
                new ObjectId(),
                username,
                hashedPassword,
                new List<string>(),
                new List<string>(),
                new List<string>(),
                new List<string>(),
                false);
            await _usersCollection.InsertOneAsync(user);
            return user;
        }

        public async Task<User> GetOneUserAsync(string id)
        {
            User user = await _usersCollection.Find(user => user._id == new ObjectId(id)).FirstAsync();
            User transmittedUser = new User(
                        user._id,
                        user.username,
                        user.likedFilmsId,
                        user.dislikedFilmsId,
                        user.opinionsId,
                        user.likedOpinionsId,
                        user.isAdmin);
            return transmittedUser;
        }

        public async Task<User> LikeOrDislikeItemAsync(string id, List<string> likedFilmsId, List<string> dislikedFilmsId)
        {
            var filter = Builders<User>.Filter.Eq(user => user._id, new ObjectId(id));
            var update = Builders<User>.Update.Set(user => user.likedFilmsId, likedFilmsId).Set(user => user.dislikedFilmsId, dislikedFilmsId);
            var options = new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After };
            User user = await _usersCollection.FindOneAndUpdateAsync(filter, update, options);
            return new User(
                user._id,
                user.username,
                user.likedFilmsId,
                user.dislikedFilmsId,
                user.opinionsId,
                user.likedOpinionsId,
                user.isAdmin
                );
        }

        public async Task<User> CreatedOpinionAsync(string userId, string opinionId)
        {
            var filter = Builders<User>.Filter.Eq(user => user._id, new ObjectId(userId));
            var update = Builders<User>.Update.Push(user => user.opinionsId, opinionId);
            var options = new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After };
            User user = await _usersCollection.FindOneAndUpdateAsync(filter, update, options);
            return new User(
                user._id,
                user.username,
                user.likedFilmsId,
                user.dislikedFilmsId,
                user.opinionsId,
                user.likedOpinionsId,
                user.isAdmin);
        }

        public async Task<User> LikeOrDislikeOpinionAsync(string userId, List<string> opinionsId, int operation)
        {
            var filter = Builders<User>.Filter.Eq(user => user._id, new ObjectId(userId));
            var update = Builders<User>.Update.Set(user => user.likedOpinionsId, opinionsId);            
            var options = new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After };
            User user = await _usersCollection.FindOneAndUpdateAsync(filter, update, options);
            return new User(
                user._id,
                user.username,
                user.likedFilmsId,
                user.dislikedFilmsId,
                user.opinionsId,
                user.likedOpinionsId,
                user.isAdmin);
        }

        public async Task<User> RemoveOpinionAsync(List<string> newOpinionsId, string userId)
        {
            var filter = Builders<User>.Filter.Eq(user => user._id, new ObjectId(userId));
            var update = Builders<User>.Update.Set(user => user.opinionsId, newOpinionsId);
            var options = new FindOneAndUpdateOptions<User> { ReturnDocument = ReturnDocument.After };
            return await _usersCollection.FindOneAndUpdateAsync(filter, update, options);
        }
    }
}
