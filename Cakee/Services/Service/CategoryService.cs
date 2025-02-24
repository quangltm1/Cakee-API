using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

public class CategoryService : ICategoryService
{
    private readonly IMongoCollection<Category> _categoryCollection;
    private readonly IMongoCollection<Cake> _cakeCollection;
    private readonly IOptions<DatabaseSettings> _dbSettings;

    public CategoryService(IOptions<DatabaseSettings> dbSettings, MongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
        _categoryCollection = database.GetCollection<Category>(dbSettings.Value.CategoriesCollectionName);
        _cakeCollection = database.GetCollection<Cake>(dbSettings.Value.CakesCollectionName); // Added cake collection if needed
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

    public async Task<string> GetByNameByIdAsync(string id)
    {
        // Query the Category collection by the provided id
        var category = await _categoryCollection.Find<Category>(category => category.Id.ToString() == id).FirstOrDefaultAsync();

        if (category == null)
        {
            return null; // If no category found, return null or an appropriate value
        }

        return category.CategoryName; // Return only the CategoryName
    }

    public async Task UpdateAsync(string id, Category category)
    {
        await _categoryCollection.ReplaceOneAsync(c => c.Id.ToString() == id, category);
    }

    public async Task<Category> GetByNameAsync(string name)
    {
        return await _categoryCollection
            .Find(category => category.CategoryName == name)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Category>> GetCakesByUserIdAsync(string storeId)
    {
        var objectId = ObjectId.Parse(storeId); // Chuyển đổi storeId từ string sang ObjectId
        return await _categoryCollection.Find(c => c.UserId == objectId).ToListAsync();
    }
}
