﻿using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

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
        var objectId = ObjectId.Parse(userId);
        var cart = await _cartCollection.Find(c => c.UserId == userId).FirstOrDefaultAsync();
        if (cart == null)
        {
            cart = new Cart
            {
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
        var existingItem = cart.Items.FirstOrDefault(i => i.CakeId == item.CakeId && i.AccessoryId == item.AccessoryId);

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

        cart.TotalPrice = cart.Items.Sum(i => i.Total);
        await _cartCollection.ReplaceOneAsync(c => c.UserId == userId, cart);

        return cart;
    }

    public async Task<bool> RemoveFromCartAsync(string userId, string productId)
    {
        var cart = await GetCartByUserIdAsync(userId);
        var itemToRemove = cart.Items.FirstOrDefault(i => i.CakeId == productId || i.AccessoryId == productId);

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
        var existingItem = cart.Items.FirstOrDefault(i => i.CakeId == item.CakeId && i.AccessoryId == item.AccessoryId);

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

}
