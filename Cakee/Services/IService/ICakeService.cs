using Cakee.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cakee.Services.IService
{
    public interface ICakeService
    {
        Task<List<Cake>> GetAllAsync();
        Task<Cake> GetByIdAsync(string id);
        Task<Cake> CreateAsync(Cake cake);
        Task UpdateAsync(string id, Cake cake);
        Task DeleteAsync(string id);
        Task<Cake> GetByNameAsync(string cakeName);
        Task<Category> GetCategoryByCakeIdAsync(string cakeId);
        Task<Cake> GetBySizeAsync(string sizeName);
        Task<List<Cake>> GetCakesByUserIdAsync(string userId);
        Task<List<Cake>> GetCakesByCategoryIdAsync(string categoryId);

    }
}

