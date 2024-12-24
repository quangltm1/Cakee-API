using Cakee.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Cakee.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IOptions<DatabaseSettings> _dbSettings;

        public UserService(IOptions<DatabaseSettings> dbSettings, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _userCollection = database.GetCollection<User>(dbSettings.Value.UsersCollectionName);
        }

        public async Task<User> CreateAsync(User user)
        {
            await _userCollection.InsertOneAsync(user);
            return user;
        }

        public async Task DeleteAsync(string id)
        {
            await _userCollection.DeleteOneAsync(user => user.Id.ToString() == id);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _userCollection.Find(user => true).ToListAsync();
        }

        public Task<User> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(string id, User user)
        {
            throw new NotImplementedException();
        }
    }
}
