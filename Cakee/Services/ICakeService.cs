using Cakee.Models;

namespace Cakee.Services
{
    public interface ICakeService
    {
        Task<List<Cake>> GetAllAsync();
        Task<Cake> GetByIdAsync(string id);
        Task<Cake> CreateAsync(Cake cake);
        Task UpdateAsync(string id, Cake cake);
        Task DeleteAsync(string id);
        Task<Cake> GetByNameAsync(string cakeName);
    }
}
