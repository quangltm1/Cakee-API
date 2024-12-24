using Cakee.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Cakee.Services
{
    public class CakeService : ICakeService
    {
        private readonly IMongoCollection<Cake> _cakeCollection;
        private readonly IOptions<DatabaseSettings> _dbSettings;

        public CakeService(IOptions<DatabaseSettings> dbSettings, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _cakeCollection = database.GetCollection<Cake>(dbSettings.Value.CakesCollectionName);

        }
        public async Task<Cake> CreateAsync(Cake cake)
        {
            await _cakeCollection.InsertOneAsync(cake);
            return cake;
        }

        public async Task DeleteAsync(string id)
        {
            await _cakeCollection.DeleteOneAsync(cake => cake.Id.ToString() == id);
        }

        public async Task<List<Cake>> GetAllAsync()
        {
            return await _cakeCollection.Find(cake => true).ToListAsync();
        }

        public async Task<Cake> GetByIdAsync(string id)
        {
            return await _cakeCollection.Find(cake => cake.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<Cake> GetByNameAsync(string cakeName)
        {
            return await _cakeCollection.Find(cake => cake.CakeName == cakeName).FirstOrDefaultAsync();
        }



        public async Task UpdateAsync(string id, Cake cake)
        {
            await _cakeCollection.ReplaceOneAsync(c => c.Id.ToString() == id, cake);
        }
    }
}
