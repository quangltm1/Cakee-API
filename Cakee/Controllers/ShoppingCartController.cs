using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cakee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _shoppingCartService.GetCartByUserIdAsync(userId);
            return Ok(cart);
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] CartItem item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _shoppingCartService.AddToCartAsync(userId, item);
            return Ok(cart);
        }

        [HttpDelete("RemoveFromCart/{productId}")]
        public async Task<IActionResult> RemoveFromCart(string productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _shoppingCartService.RemoveFromCartAsync(userId, productId);
            return Ok(result);
        }

        [HttpDelete("ClearCart")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _shoppingCartService.ClearCartAsync(userId);
            return Ok(result);
        }

        [HttpPut("UpdateCartItem")]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartItem item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _shoppingCartService.UpdateCartItemAsync(userId, item);
            return Ok(cart);
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _shoppingCartService.ClearCartAsync(userId);
            return Ok(result);
        }

        [HttpGet("GetCartByUserId/{userId}")]
        public async Task<IActionResult> GetCartByUserId(string userId)
        {
            var cart = await _shoppingCartService.GetCartByUserIdAsync(userId);
            return Ok(cart);
        }

        [HttpPost("AddToCartByUserId/{userId}")]
        public async Task<IActionResult> AddToCartByUserId(string userId, [FromBody] CartItem item)
        {
            var cart = await _shoppingCartService.AddToCartAsync(userId, item);
            return Ok(cart);
        }

        [HttpDelete("RemoveFromCartByUserId/{userId}/{productId}")]
        public async Task<IActionResult> RemoveFromCartByUserId(string userId, string productId)
        {
            var result = await _shoppingCartService.RemoveFromCartAsync(userId, productId);
            return Ok(result);
        }

        [HttpDelete("ClearCartByUserId/{userId}")]
        public async Task<IActionResult> ClearCartByUserId(string userId)
        {
            var result = await _shoppingCartService.ClearCartAsync(userId);
            return Ok(result);
        }

        [HttpPut("UpdateCartItemByUserId/{userId}")]
        public async Task<IActionResult> UpdateCartItemByUserId(string userId, [FromBody] CartItem item)
        {
            var cart = await _shoppingCartService.UpdateCartItemAsync(userId, item);
            return Ok(cart);
        }

        [HttpPost("CheckoutByUserId/{userId}")]
        public async Task<IActionResult> CheckoutByUserId(string userId)
        {
            var result = await _shoppingCartService.ClearCartAsync(userId);
            return Ok(result);
        }
    }
}
