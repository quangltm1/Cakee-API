using Cakee.Models;
using Cakee.Services.IService;
using Cakee.Services.Service;
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
        private readonly ICakeService _cakeService;
        private readonly IAcessoryService _acessoryService;

        public ShoppingCartController(IShoppingCartService shoppingCartService, ICakeService cakeService, IAcessoryService acessoryService)
        {
            _shoppingCartService = shoppingCartService;
            _cakeService = cakeService;
            _acessoryService = acessoryService;
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
            var cake = await _cakeService.GetByIdAsync(item.CakeId ?? "");
            var accessory = await _acessoryService.GetByIdAsync(item.AcessoryId ?? "");

            if (cake == null && accessory == null)
                return BadRequest("Sản phẩm không tồn tại!");

            // ✅ Tính giá của sản phẩm (nếu có)
            decimal cakePrice = cake != null ? cake.CakePrice : 0;
            decimal accessoryPrice = accessory != null ? accessory.AcessoryPrice : 0;

            // ✅ Cập nhật tổng tiền của sản phẩm
            item.Total = (cakePrice * item.QuantityCake) + (accessoryPrice * item.QuantityAccessory);

            // ✅ Thêm vào giỏ hàng
            var cart = await _shoppingCartService.AddToCartAsync(userId, item);

            // ✅ Cập nhật tổng tiền của giỏ hàng
            cart.TotalPrice = cart.Items.Sum(i => i.Total);
            await _shoppingCartService.UpdateCartAsync(userId, cart.Items);

            return Ok(cart);
        }


        [HttpDelete("RemoveFromCart/{productId}")]
        public async Task<IActionResult> RemoveFromCart(string productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _shoppingCartService.GetCartByUserIdAsync(userId);

            if (cart == null)
                return NotFound("Giỏ hàng không tồn tại!");

            var itemToRemove = cart.Items.FirstOrDefault(i => i.CakeId == productId);
            if (itemToRemove == null)
                return NotFound("Sản phẩm không tồn tại trong giỏ hàng!");

            // ✅ Xóa sản phẩm khỏi giỏ hàng
            cart.Items.Remove(itemToRemove);

            // ✅ Cập nhật lại tổng tiền của giỏ hàng
            cart.TotalPrice = cart.Items.Sum(i => i.Total);

            // ✅ Lưu thay đổi vào database
            await _shoppingCartService.UpdateCartAsync(userId, cart.Items);

            return Ok(cart);
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
            var cart = await _shoppingCartService.GetCartByUserIdAsync(userId);

            if (cart == null)
                return NotFound("Giỏ hàng không tồn tại!");

            var existingItem = cart.Items.FirstOrDefault(i => i.CakeId == item.CakeId);
            if (existingItem == null)
                return NotFound("Sản phẩm không tồn tại trong giỏ hàng!");

            // ✅ Cập nhật số lượng
            existingItem.QuantityCake = item.QuantityCake;
            existingItem.QuantityAccessory = item.QuantityAccessory;

            // ✅ Lấy giá sản phẩm từ database
            var cake = await _cakeService.GetByIdAsync(existingItem.CakeId);
            var accessory = await _acessoryService.GetByIdAsync(existingItem.AcessoryId ?? "");

            decimal cakePrice = cake != null ? cake.CakePrice : 0;
            decimal accessoryPrice = accessory != null ? accessory.AcessoryPrice : 0;

            // ✅ Tính lại tổng tiền của sản phẩm này
            existingItem.Total = (cakePrice * existingItem.QuantityCake) + (accessoryPrice * existingItem.QuantityAccessory);

            // ✅ Tính lại tổng tiền của giỏ hàng
            cart.TotalPrice = cart.Items.Sum(i => i.Total);

            // ✅ Lưu thay đổi vào database
            await _shoppingCartService.UpdateCartAsync(userId, cart.Items);

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

            // Lấy danh sách bánh và phụ kiện từ Database
            var cakesTask = _cakeService.GetAllAsync();
            var accessoriesTask = _acessoryService.GetAllAsync();
            await Task.WhenAll(cakesTask, accessoriesTask);

            var cakes = cakesTask.Result;
            var accessories = accessoriesTask.Result;

            // Cập nhật thông tin giỏ hàng
            var updatedItems = cart.Items.Select(item => new
            {
                item.CakeId,
                CakeName = cakes.FirstOrDefault(c => c.Id.ToString() == item.CakeId)?.CakeName ?? "Không xác định",
                item.AcessoryId,
                AccessoryName = accessories.FirstOrDefault(a => a.Id.ToString() == item.AcessoryId)?.AcessoryName ?? "Không xác định",
                item.QuantityCake,
                item.QuantityAccessory,
                item.Total
            }).ToList();

            var updatedCart = new
            {
                cart.Id,
                cart.UserId,
                Items = updatedItems,
                cart.TotalPrice
            };

            return Ok(updatedCart);
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
