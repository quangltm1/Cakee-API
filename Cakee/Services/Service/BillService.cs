using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Cakee.Services.Service
{
    public class BillService : IBillService
    {
        private readonly IMongoCollection<Bill> _billcollection;
        private readonly IMongoCollection<Cake> _cakeCollection;
        private readonly IOptions<DatabaseSettings> _dbSettings;

        public BillService(IOptions<DatabaseSettings> dbSettings, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _billcollection = database.GetCollection<Bill>(dbSettings.Value.BillsCollectionName);
            _cakeCollection = database.GetCollection<Cake>(dbSettings.Value.CakesCollectionName);
        }

        public async Task<List<Bill>> GetBillByCustomId(string customId) => await _billcollection.Find(bill => bill.BillCustomId == customId).ToListAsync();

        public async Task<Bill> CreateAsync(Bill bill)
        {
            await _billcollection.InsertOneAsync(bill);
            return bill;
        }

        public async Task DeleteAsync(string id)
        {
            await _billcollection.DeleteOneAsync(bill => bill.Id.ToString() == id);
        }

        public async Task<List<Bill>> GetAllAsync()
        {
            var bills = await _billcollection.Find(bill => true).ToListAsync();
            return bills;
        }

        public async Task<Bill> GetByIdAsync(string id)
        {
            return await _billcollection.Find(bill => bill.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(string id, Bill bill)
        {
            await _billcollection.ReplaceOneAsync(c => c.Id.ToString() == id, bill);
        }

    }
}
