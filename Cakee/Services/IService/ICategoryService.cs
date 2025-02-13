using Cakee.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cakee.Services.IService
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(string id);
        Task<Category> CreateAsync(Category category);
        Task UpdateAsync(string id, Category category);
        Task DeleteAsync(string id);
        Task<Category> GetByNameAsync(string categoryName);
        Task<string> GetByNameByIdAsync(string id);
    }
}
