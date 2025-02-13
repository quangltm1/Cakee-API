﻿using Cakee.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Cakee.Services
{
    public class CakeSizeService : ICakeSizeService
    {
        private readonly IMongoCollection<CakeSize> _cakesizecollection;
        private readonly IMongoCollection<Cake> _cakeCollection;
        private readonly IOptions<DatabaseSettings> _dbSettings;

        public CakeSizeService(IOptions<DatabaseSettings> dbSettings, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _cakesizecollection = database.GetCollection<CakeSize>(dbSettings.Value.CakeSizesCollectionName);
            _cakeCollection = database.GetCollection<Cake>(dbSettings.Value.CakesCollectionName);
        }
        public async Task<CakeSize> CreateAsync(CakeSize cakeSize)
        {
            await _cakesizecollection.InsertOneAsync(cakeSize);
            return cakeSize;
        }

        public async Task DeleteAsync(string id)
        {
            await _cakesizecollection.DeleteOneAsync(cakeSize => cakeSize.Id.ToString() == id);
        }
        public async Task<List<CakeSize>> GetAllAsync()
        {
            var cakesizes = await _cakesizecollection.Find(cakeSize => true).ToListAsync();
            return cakesizes;
        }
        public async Task<CakeSize> GetByIdAsync(string id)
        {
            return await _cakesizecollection.Find(cakeSize => cakeSize.Id.ToString() == id).FirstOrDefaultAsync();
        }
        public async Task UpdateAsync(string id, CakeSize cakeSize)
        {
            await _cakesizecollection.ReplaceOneAsync(cakeSize => cakeSize.Id.ToString() == id, cakeSize);
        }
    }
}
