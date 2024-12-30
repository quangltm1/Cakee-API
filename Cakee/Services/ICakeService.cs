using Cakee.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cakee.Services
{
    public interface ICakeService
    {
        Task<List<Cake>> GetAllAsync(); // Updated to return a specific type
        Task<Cake> GetByIdAsync(string id);
        Task<Cake> CreateAsync(Cake cake);
        Task UpdateAsync(string id, Cake cake);
        Task DeleteAsync(string id);
        Task<Cake> GetByNameAsync(string cakeName);
        Task<Category> GetCategoryByCakeIdAsync(string cakeId);
    }
}

