using Cakee.Models;

namespace Cakee.Services
{
    public interface ICakeSizeService
    {
        Task<List<CakeSize>> GetAllAsync();
        Task<CakeSize> GetByIdAsync(string id);
        Task<CakeSize> CreateAsync(CakeSize cakeSize);
        Task UpdateAsync(string id, CakeSize cakeSize);
        Task DeleteAsync(string id);
    }
}
