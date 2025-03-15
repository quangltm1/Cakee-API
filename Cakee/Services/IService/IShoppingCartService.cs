using Cakee.Models;

namespace Cakee.Services.IService
{
    public interface IShoppingCartService
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<Cart> AddToCartAsync(string userId, CartItem item);
        Task<bool> RemoveFromCartAsync(string userId, string productId);
        Task<bool> ClearCartAsync(string userId);
        Task<Cart> UpdateCartItemAsync(string userId, CartItem item);
        Task<bool> ProductExistsAsync(string productId);
        Task<bool> UpdateCartAsync(string userId, List<CartItem> updatedItems);

    }
}
