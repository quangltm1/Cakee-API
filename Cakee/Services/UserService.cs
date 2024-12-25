using Cakee.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Data;

namespace Cakee.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserService(IOptions<DatabaseSettings> dbSettings, MongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _userCollection = database.GetCollection<User>(dbSettings.Value.UsersCollectionName);
        }

        public async Task<List<User>> GetAllAsync()
        {
            try
            {
                return await _userCollection.Find(_ => true).ToListAsync();
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

        public async Task<User> CreateAsync(User user)
        {
            try
            {
                await _userCollection.InsertOneAsync(user);
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                throw new ApplicationException("Unable to create user at this time.", ex);
            }
        }

        public async Task UpdateAsync(string id, User user)
        {
            try
            {
                // Ensure the user exists
                var existingUser = await _userCollection.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();
                if (existingUser == null)
                {
                    throw new ApplicationException($"No user found with ID {id} to update.");
                }

                // Update only the modified fields
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
                // Ensure the ID is properly parsed to ObjectId
                var objectId = new ObjectId(id);

                // Create a filter for the user with the given ID
                var filter = Builders<User>.Filter.Eq(user => user.Id, objectId);

                // Perform the delete operation
                var result = await _userCollection.DeleteOneAsync(filter);

                // Check if no documents were deleted
                if (result.DeletedCount == 0)
                {
                    throw new ApplicationException($"No user found with ID {id} to delete.");
                }
            }
            catch (Exception ex)
            {
                // Log and rethrow the exception
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
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    Role = user.Role
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users by role: {ex.Message}");
                throw new ApplicationException($"Unable to fetch users with role {role}.", ex);
            }
        }
    }
}
