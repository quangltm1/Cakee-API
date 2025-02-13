using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Cakee.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<RefreshToken> _refreshTokenCollection;


        public UserService(IOptions<DatabaseSettings> dbSettings, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _userCollection = database.GetCollection<User>(dbSettings.Value.UsersCollectionName);
            _refreshTokenCollection = database.GetCollection<RefreshToken>(dbSettings.Value.RefreshTokensCollectionName);
        }

        public async Task<List<User>> GetAllAsync()
        {
            try
            {
                return await _userCollection.Find(_ => true).ToListAsync();
            }
            catch (MongoConnectionException ex)
            {
                Console.WriteLine($"MongoDB connection error: {ex.Message}");
                throw new ApplicationException("Unable to connect to the database server. Please try again later.", ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Timeout error: {ex.Message}");
                throw new ApplicationException("The request timed out. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all users: {ex.Message}");
                throw new ApplicationException("Unable to fetch users at this time.", ex);
            }
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _userCollection.Find(user => user.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUserNameAsync(string username)
        {
            return await _userCollection.Find(user => user.UserName == username).FirstOrDefaultAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                user.Role = 0;
                await _userCollection.InsertOneAsync(user);
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tạo người dùng: {ex.Message}");
                throw new ApplicationException("Không thể tạo người dùng tại thời điểm này.", ex);
            }
        }

        public async Task<User> CreateAdminAsync(User user)
        {
            try
            {
                user.Role = 1;
                await _userCollection.InsertOneAsync(user);
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tạo admin: {ex.Message}");
                throw new ApplicationException("Không thể tạo admin tại thời điểm này.", ex);
            }
        }

        public async Task UpdateAsync(string id, User user)
        {
            try
            {
                var existingUser = await _userCollection.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();
                if (existingUser == null)
                {
                    throw new ApplicationException($"No user found with ID {id} to update.");
                }

                var update = Builders<User>.Update
                    .Set(u => u.UserName, user.UserName)
                    .Set(u => u.FullName, user.FullName)
                    .Set(u => u.Phone, user.Phone)
                    .Set(u => u.Role, user.Role);

                var filter = Builders<User>.Filter.Eq(u => u.Id, new ObjectId(id));
                var result = await _userCollection.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                {
                    throw new ApplicationException($"No user found with ID {id} to update.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw new ApplicationException($"Unable to update user with ID {id}.", ex);
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                var objectId = new ObjectId(id);
                var filter = Builders<User>.Filter.Eq(user => user.Id, objectId);
                var result = await _userCollection.DeleteOneAsync(filter);

                if (result.DeletedCount == 0)
                {
                    throw new ApplicationException($"No user found with ID {id} to delete.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                throw new ApplicationException($"Unable to delete user with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<object>> GetByRoleAsync(int role)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(user => user.Role, role);
                var users = await _userCollection.Find(filter).ToListAsync();

                if (!users.Any())
                {
                    Console.WriteLine($"No users found with role = {role}");
                    return Enumerable.Empty<object>();
                }

                return users.Select(user => new
                {
                    Id = user.Id.ToString(),
                    user.UserName,
                    user.FullName,
                    user.Phone,
                    user.Role
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users by role: {ex.Message}");
                throw new ApplicationException($"Unable to fetch users with role {role}.", ex);
            }
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshTokenModel)
        {
            try
            {
                await _refreshTokenCollection.InsertOneAsync(refreshTokenModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving refresh token: {ex.Message}");
                throw new ApplicationException("Unable to save refresh token at this time.", ex);
            }
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            return await _refreshTokenCollection.Find(rt => rt.Token == token).FirstOrDefaultAsync();
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _refreshTokenCollection.ReplaceOneAsync(rt => rt.Id == refreshToken.Id, refreshToken);
        }
    }
}
