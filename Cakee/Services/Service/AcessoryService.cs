using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cakee.Services.Service
{
    public class AcessoryService : IAcessoryService
    {
        private readonly IMongoCollection<Acessory> _acessoryCollection;
        private readonly IOptions<DatabaseSettings> _dbSettings;

        public AcessoryService(IOptions<DatabaseSettings> dbSettings, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _acessoryCollection = database.GetCollection<Acessory>(dbSettings.Value.AcessorysCollectionName);
        }

        public async Task<Acessory> CreateAsync(Acessory acessory)
        {
            await _acessoryCollection.InsertOneAsync(acessory);
            return acessory;
        }

        public async Task DeleteAsync(string id)
        {
            await _acessoryCollection.DeleteOneAsync(acessory => acessory.Id.ToString() == id);
        }

        public async Task<List<Acessory>> GetAllAsync()
        {
            var acessories = await _acessoryCollection.Find(acessory => true).ToListAsync();
            return acessories;
        }

        public async Task<Acessory> GetByIdAsync(string id)
        {
            return await _acessoryCollection.Find(acessory => acessory.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(string id, Acessory acessory)
        {
            await _acessoryCollection.ReplaceOneAsync(c => c.Id.ToString() == id, acessory);
        }

        public async Task<Acessory> GetByNameAsync(string? acessoryName)
        {
            return await _acessoryCollection.Find(acessory => acessory.AcessoryName == acessoryName).FirstOrDefaultAsync();
        }

        public async Task<List<Acessory>> GetAcessoryByUserIdAsync(string storeId)
        {
            var objectId = ObjectId.Parse(storeId); // Chuyển đổi storeId từ string sang ObjectId
            return await _acessoryCollection.Find(c => c.UserId == objectId.ToString()).ToListAsync();
        }
    }
}
