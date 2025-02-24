using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cakee.Services.Service
{
    public class CakeService : ICakeService
    {
        private readonly IMongoCollection<Cake> _cakeCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IOptions<DatabaseSettings> _dbSettings;

        public CakeService(IOptions<DatabaseSettings> dbSettings, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _cakeCollection = database.GetCollection<Cake>(dbSettings.Value.CakesCollectionName);
            _categoryCollection = database.GetCollection<Category>(dbSettings.Value.CategoriesCollectionName);

        }

        public async Task<Category?> GetCategoryByCakeIdAsync(string cakeId)
        {
            var cake = await _cakeCollection.Find(c => c.Id.ToString() == cakeId).FirstOrDefaultAsync();

            if (cake != null)
            {
                var category = await _categoryCollection.Find(c => c.Id == cake.CakeCategoryId).FirstOrDefaultAsync();
                return category ?? new Category { Id = ObjectId.Empty, CategoryName = "Không xác định" };
            }

            return null;
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
            var cakes = await _cakeCollection.Find(cake => true).ToListAsync();
            return cakes;

        }

        public async Task<Cake> GetByIdAsync(string id)
        {
            return await _cakeCollection.Find(cake => cake.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<Cake> GetByNameAsync(string cakeName)
        {
            return await _cakeCollection.Find(cake => cake.CakeName == cakeName).FirstOrDefaultAsync();
        }

        public async Task<Cake> GetBySizeAsync(string sizeName)
        {
            return await _cakeCollection.Find(cake => cake.CakeSize.ToString() == sizeName).FirstOrDefaultAsync();
        }


        public async Task UpdateAsync(string id, Cake cake)
        {
            var update = Builders<Cake>.Update
                .Set(c => c.CakeName, cake.CakeName)
                .Set(c => c.CakeDescription, cake.CakeDescription)
                .Set(c => c.CakePrice, cake.CakePrice)
                .Set(c => c.CakeSize, cake.CakeSize)
                .Set(c => c.CakeImage, cake.CakeImage)
                .Set(c => c.CakeQuantity, cake.CakeQuantity);

            // Kiểm tra và cập nhật CakeCategoryId nếu khác null
            if (cake.CakeCategoryId != ObjectId.Empty)
            {
                update = update.Set(c => c.CakeCategoryId, cake.CakeCategoryId);
            }

            await _cakeCollection.UpdateOneAsync(c => c.Id == ObjectId.Parse(id), update);
        }


        public async Task<List<Cake>> GetCakesByUserIdAsync(string storeId)
        {
            var objectId = ObjectId.Parse(storeId); // Chuyển đổi storeId từ string sang ObjectId
            return await _cakeCollection.Find(c => c.UserId == objectId).ToListAsync();
        }


    }
}
