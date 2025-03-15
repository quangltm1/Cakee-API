using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IMongoCollection<Cart> _cartCollection;

    public ShoppingCartService(IOptions<DatabaseSettings> dbSettings, MongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
        _cartCollection = database.GetCollection<Cart>(dbSettings.Value.CartsCollectionName);
    }

    public async Task<Cart> GetCartByUserIdAsync(string userId)
    {
        var filter = Builders<Cart>.Filter.Eq(c => c.UserId, userId);
        var cart = await _cartCollection.Find(filter).FirstOrDefaultAsync();

        if (cart == null)
        {
            cart = new Cart
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserId = userId,
                Items = new List<CartItem>()
            };
            await _cartCollection.InsertOneAsync(cart);
        }
        return cart;
    }

    public async Task<Cart> AddToCartAsync(string userId, CartItem item)
    {
        var cart = await GetCartByUserIdAsync(userId);
        var existingItem = cart.Items.FirstOrDefault(i => i.CakeId == item.CakeId && i.AcessoryId == item.AcessoryId);

        if (existingItem != null)
        {
            existingItem.QuantityCake += item.QuantityCake;
            existingItem.QuantityAccessory += item.QuantityAccessory;
            existingItem.Total += item.Total;
        }
        else
        {
            cart.Items.Add(item);
        }

        // ✅ Cập nhật tổng tiền giỏ hàng
        cart.TotalPrice = cart.Items.Sum(i => i.Total);

        await _cartCollection.ReplaceOneAsync(c => c.UserId == userId, cart);
        return cart;
    }


    public async Task<bool> RemoveFromCartAsync(string userId, string productId)
    {
        var cart = await GetCartByUserIdAsync(userId);
        var itemToRemove = cart.Items.FirstOrDefault(i => i.CakeId == productId || i.AcessoryId == productId);

        if (itemToRemove != null)
        {
            cart.Items.Remove(itemToRemove);
            cart.TotalPrice = cart.Items.Sum(i => i.Total);
            await _cartCollection.ReplaceOneAsync(c => c.UserId == userId, cart);
            return true;
        }
        return false;
    }

    public async Task<bool> ClearCartAsync(string userId)
    {
        await _cartCollection.DeleteOneAsync(c => c.UserId == userId);
        return true;
    }

    public async Task<Cart> UpdateCartItemAsync(string userId, CartItem item)
    {
        var cart = await GetCartByUserIdAsync(userId);
        var existingItem = cart.Items.FirstOrDefault(i => i.CakeId == item.CakeId && i.AcessoryId == item.AcessoryId);

        if (existingItem != null)
        {
            existingItem.QuantityCake = item.QuantityCake;
            existingItem.QuantityAccessory = item.QuantityAccessory;
            existingItem.Total = item.Total;
        }
        else
        {
            cart.Items.Add(item);
        }

        cart.TotalPrice = cart.Items.Sum(i => i.Total);
        await _cartCollection.ReplaceOneAsync(c => c.UserId == userId, cart);

        return cart;
    }

    public async Task<bool> ProductExistsAsync(string productId)
    {
        var filter = Builders<Cart>.Filter.ElemMatch(c => c.Items, i => i.CakeId == productId || i.AcessoryId == productId);
        var cart = await _cartCollection.Find(filter).FirstOrDefaultAsync();
        return cart != null;
    }


    public async Task<bool> UpdateCartAsync(string userId, List<CartItem> updatedItems)
    {
        var filter = Builders<Cart>.Filter.Eq(c => c.UserId, userId);
        var update = Builders<Cart>.Update
            .Set(c => c.Items, updatedItems)
            .Set(c => c.TotalPrice, updatedItems.Sum(i => i.Total));

        var result = await _cartCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

}