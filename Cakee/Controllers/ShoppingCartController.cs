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

            // ✅ Kiểm tra sản phẩm tồn tại trước khi thêm
            var productExists = await _shoppingCartService.ProductExistsAsync(item.CakeId ?? item.AccessoryId);
            if (!productExists)
                return BadRequest("Sản phẩm không tồn tại!");

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
            var cart = await _shoppingCartService.GetCartByUserIdAsync(userId);

            if (cart == null || cart.Items.Count == 0)
                return BadRequest("Giỏ hàng trống, không thể thanh toán!");

            if (cart.TotalPrice <= 0)
                return BadRequest("Tổng giá trị giỏ hàng không hợp lệ!");

            // ✅ Xử lý thanh toán ở đây (VD: trừ hàng, tạo đơn hàng)
            await _shoppingCartService.ClearCartAsync(userId);
            return Ok(new { message = "Thanh toán thành công!" });
        }

        [HttpGet("GetCartByUserId/{userId}")]
        public async Task<IActionResult> GetCartByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID không hợp lệ");

            var cart = await _shoppingCartService.GetCartByUserIdAsync(userId);
            if (cart == null)
                return NotFound("Không tìm thấy giỏ hàng cho userId này");

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
            var cart = await _shoppingCartService.GetCartByUserIdAsync(userId);

            if (cart == null || cart.Items.Count == 0)
                return BadRequest("Giỏ hàng trống, không thể thanh toán!");

            await _shoppingCartService.ClearCartAsync(userId);
            return Ok(new { message = "Thanh toán thành công!" });
        }

        [HttpGet("GetCartItemCount")]
        public async Task<IActionResult> GetCartItemCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _shoppingCartService.GetCartByUserIdAsync(userId);
            int totalItems = cart?.Items?.Sum(item => item.QuantityCake + item.QuantityAccessory) ?? 0;
            return Ok(new { totalItems });
        }

        [HttpPut("UpdateCartTotal")]
        public async Task<IActionResult> UpdateCartTotal()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _shoppingCartService.GetCartByUserIdAsync(userId);

            if (cart == null) return NotFound("Giỏ hàng không tồn tại!");

            cart.TotalPrice = cart.Items.Sum(i => i.Total);
            await _shoppingCartService.UpdateCartAsync(userId, cart.Items);

            return Ok(cart);
        }
    }
}
