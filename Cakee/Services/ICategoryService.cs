using Cakee.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cakee.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(string id);
        Task<Category> CreateAsync(Category category);
        Task UpdateAsync(string id, Category category);
        Task DeleteAsync(string id);
        Task<Category> GetByNameAsync(string categoryName);
    }
}
