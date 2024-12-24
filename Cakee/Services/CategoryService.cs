using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Cakee.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cakee.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IOptions<DatabaseSettings> _dbSettings;

        public CategoryService(IOptions<DatabaseSettings> dbSettings, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _categoryCollection = database.GetCollection<Category>(dbSettings.Value.CategoriesCollectionName);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            await _categoryCollection.InsertOneAsync(category);
            return category;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryCollection.Find(category => true).ToListAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await _categoryCollection.DeleteOneAsync(category => category.Id.ToString() == id);
        }

        public async Task<Category> GetByIdAsync(string id)
        {
            return await _categoryCollection.Find<Category>(category => category.Id.ToString() == id).FirstOrDefaultAsync();
        }

        // Fixed method signature
        public async Task UpdateAsync(string id, Category category)
        {
            await _categoryCollection.ReplaceOneAsync(c => c.Id.ToString() == id, category);
        }

        // Fixed GetByNameAsync method for MongoDB
        public async Task<Category> GetByNameAsync(string name)
        {
            return await _categoryCollection
                .Find(category => category.CategoryName == name)
                .FirstOrDefaultAsync();
        }
    }
}
