using Cakee.Models;
using Cakee.Services.IService;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cakee.Services.Service
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IMongoCollection<Cart> _cartCollection;

        public ShoppingCartService(IMongoDatabase database)
        {
            _cartCollection = database.GetCollection<Cart>("Carts");
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            var cart = await _cartCollection.Find(c => c.UserId.ToString() == userId).FirstOrDefaultAsync();
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = ObjectId.Parse(userId),
                    Items = new List<CartItem>()
                };
                await _cartCollection.InsertOneAsync(cart);
            }
            return cart;
        }

        public async Task<Cart> AddToCartAsync(string userId, CartItem item)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId && i.ProductType == item.ProductType);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }

            cart.TotalPrice = cart.Items.Sum(i => i.Quantity * i.Price);
            await _cartCollection.ReplaceOneAsync(c => c.UserId.ToString() == userId, cart, new ReplaceOptions { IsUpsert = true });

            return cart;
        }

        public async Task<bool> RemoveFromCartAsync(string userId, string productId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            cart.Items.RemoveAll(i => i.ProductId.ToString() == productId);
            cart.TotalPrice = cart.Items.Sum(i => i.Quantity * i.Price);
            await _cartCollection.ReplaceOneAsync(c => c.UserId.ToString() == userId, cart);

            return true;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            await _cartCollection.DeleteOneAsync(c => c.UserId.ToString() == userId);
            return true;
        }

        public async Task<Cart> UpdateCartItemAsync(string userId, CartItem item)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId && i.ProductType == item.ProductType);

            if (existingItem != null)
            {
                existingItem.Quantity = item.Quantity;
            }

            cart.TotalPrice = cart.Items.Sum(i => i.Quantity * i.Price);
            await _cartCollection.ReplaceOneAsync(c => c.UserId.ToString() == userId, cart);

            return cart;
        }
    }
}
