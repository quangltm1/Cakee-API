using Cakee_Api.Datas;
using Cakee_Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Cakee_Api.Services
{
    public class CakeService : ICakeService
    {
        private readonly IMongoCollection<Cake> _cakeCollection;
        private readonly IOptions<MongoDBSettings> _dbSettings;
        private readonly List<Cake> _cakes = new List<Cake>();

        public CakeService(IOptions<MongoDBSettings> dbSettings, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _cakeCollection = database.GetCollection<Cake>(dbSettings.Value.CakesCollectionName);

        }

        public async Task<Cake> CreateAsync(Cake cake)
        {
            await _cakeCollection.InsertOneAsync(cake);
            return cake;
        }

        public async Task<List<Cake>> GetAllCakes()
        {
            return await _cakeCollection.Find(cake => true).ToListAsync();
        }

        public async Task<IEnumerable<Cake>> GetCakesWithDetailsAsync()
        {
            var pipeline = new[]
            {
        // Lookup to join with Category
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "Category" },
            { "localField", "CakeCategoryId" },
            { "foreignField", "_id" },
            { "as", "CakeCategory" }
        }),
        // Unwind to convert CakeCategory array to object
        new BsonDocument("$unwind", new BsonDocument
        {
            { "path", "$CakeCategory" },
            { "preserveNullAndEmptyArrays", true }
        }),
        // Lookup to join with CakeSize
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "CakeSize" },
            { "localField", "CakeSizeId" },
            { "foreignField", "_id" },
            { "as", "CakeSize" }
        }),
        // Unwind to convert CakeSize array to object
        new BsonDocument("$unwind", new BsonDocument
        {
            { "path", "$CakeSize" },
            { "preserveNullAndEmptyArrays", true }
        }),
        // Project the final structure and modify fields
        new BsonDocument("$project", new BsonDocument
        {
            { "_id", 1 },
            { "Name", 1 },
            { "Price", 1 },
            { "CategoryName", "$CakeCategory.Name" },
            { "CakeCategoryId", new BsonDocument("$toString", "$CakeCategoryId") }, // Chuyển ObjectId thành chuỗi
            { "CakeSizeId", new BsonDocument("$toString", "$CakeSizeId") }, // Chuyển ObjectId thành chuỗi
            { "SizeName", "$CakeSize.Name" },
            { "cakeStock", 1 },
            { "cakeRating", 1 },
            { "cakeSizePrice", 1 },
            { "billDetails", 1 }
        })
    };

            var result = await _cakeCollection.Aggregate<Cake>(pipeline).ToListAsync();
            return result;
        }








    }
}
