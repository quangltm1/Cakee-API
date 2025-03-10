using Cakee.Models;

namespace Cakee.Services.IService
{
    public interface IBillService
    {
        Task<List<Bill>> GetAllAsync();
        Task<Bill> GetByIdAsync(string id);
        Task<Bill> CreateAsync(Bill bill);
        Task UpdateAsync(string id, Bill bill);
        Task DeleteAsync(string id);

        Task<List<Bill>> GetBillByCustomId(string customId);
    }
}
