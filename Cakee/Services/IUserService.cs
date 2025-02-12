using Cakee.Models;
using MongoDB.Bson;

namespace Cakee.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User> GetByIdAsync(string id);
        Task<User> CreateUserAsync(User user);
        Task<User> CreateAdminAsync(User admin);
        Task UpdateAsync(string id, User user);
        Task DeleteAsync(string id);
        Task<IEnumerable<object>> GetByRoleAsync(int role);
        Task<User> GetUserByUserNameAsync(string userName);
        Task SaveRefreshTokenAsync(RefreshToken refreshTokenModel);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
    }

}
