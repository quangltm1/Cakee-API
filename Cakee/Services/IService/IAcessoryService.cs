using Cakee.Models;

namespace Cakee.Services.IService
{
    public interface IAcessoryService
    {
        Task<List<Acessory>> GetAllAsync();
        Task<Acessory> GetByIdAsync(string id);
        Task<Acessory> CreateAsync(Acessory acessory);
        Task UpdateAsync(string id, Acessory acessory);
        Task DeleteAsync(string id);
        Task<Acessory> GetByNameAsync(string acessoryName);
    }
}
