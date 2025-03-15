using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cakee.Services.Service
{
    public class BillService : IBillService
    {
        private readonly IMongoCollection<Bill> _billCollection;
        private readonly IMongoCollection<Cake> _cakeCollection;

        public BillService(IOptions<DatabaseSettings> dbSettings, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _billCollection = database.GetCollection<Bill>(dbSettings.Value.BillsCollectionName);
            _cakeCollection = database.GetCollection<Cake>(dbSettings.Value.CakesCollectionName);
        }

        /// ✅ **Lấy danh sách đơn hàng theo ID khách hàng**
        public async Task<List<Bill>> GetBillByCustomId(string customId) =>
            await _billCollection.Find(bill => bill.BillCustomId == customId).ToListAsync();

        /// ✅ **Tạo đơn hàng cho khách đã đăng nhập**
        public async Task<Bill> CreateAsync(Bill bill)
        {
            bill.Id = ObjectId.GenerateNewId(); // 🔥 Fix lỗi DuplicateKey: Tạo ID mới
            await _billCollection.InsertOneAsync(bill);
            return bill;
        }

        /// ✅ **Tạo đơn hàng cho khách vãng lai**
        public async Task<Bill> CreateBillForGuest(Bill bill)
        {
            bill.Id = ObjectId.GenerateNewId(); // 🔥 Fix lỗi DuplicateKey
            bill.BillStatus = BillStatus.Pending;
            bill.BillDeposit = 0;
            bill.BillReceiveDate = DateTime.Now.AddDays(3);

            await _billCollection.InsertOneAsync(bill);
            return bill;
        }

        /// ✅ **Xóa đơn hàng theo ID**
        public async Task DeleteAsync(string id)
        {
            await _billCollection.DeleteOneAsync(bill => bill.Id.ToString() == id);
        }

        /// ✅ **Lấy danh sách tất cả đơn hàng**
        public async Task<List<Bill>> GetAllAsync()
        {
            return await _billCollection.Find(bill => true).ToListAsync();
        }

        /// ✅ **Lấy đơn hàng theo ID**
        public async Task<Bill> GetByIdAsync(string id)
        {
            return await _billCollection.Find(bill => bill.Id.ToString() == id).FirstOrDefaultAsync();
        }

        /// ✅ **Cập nhật đơn hàng theo ID**
        public async Task UpdateAsync(string id, Bill bill)
        {
            await _billCollection.ReplaceOneAsync(c => c.Id.ToString() == id, bill);
        }

        /// ✅ **Cập nhật trạng thái đơn hàng**
        public async Task<bool> UpdateOrderStatusAsync(string billId, BillStatus status)
        {
            var filter = Builders<Bill>.Filter.Eq(b => b.Id, ObjectId.Parse(billId));
            var update = Builders<Bill>.Update.Set(b => b.BillStatus, status);

            var result = await _billCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}
