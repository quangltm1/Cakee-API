using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;

namespace Cakee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BillController : ControllerBase
    {
        private readonly IBillService _billService;
        private readonly IUserService _userService;
        private readonly ICakeService _cakeService;
        private readonly IAcessoryService _acessoryService;

        public BillController(IBillService billService, IUserService userService, ICakeService cakeService, IAcessoryService acessoryService)
        {
            _billService = billService;
            _userService = userService;
            _cakeService = cakeService;
            _acessoryService = acessoryService;
        }

        /// ✅ **Lấy danh sách tất cả đơn hàng**
        [HttpGet("GetAllBill")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Bill>>> GetBill()
        {
            var bills = await _billService.GetAllAsync();
            if (bills == null || bills.Count == 0)
                return NotFound("Không tìm thấy đơn hàng!");

            var response = new List<object>();

            foreach (var bill in bills)
            {
                var user = await _userService.GetByIdAsync(bill.BillCustomId ?? "");
                var cake = await _cakeService.GetByIdAsync(bill.BillCakeId ?? "");
                var acessory = await _acessoryService.GetByIdAsync(bill.BillAcessoriesId ?? "");

                response.Add(new
                {
                    Id = bill.Id.ToString(),
                    CustomName = user?.FullName ?? bill.BillDeliveryCustomName + " (Khách vãng lai)",
                    Address = bill.BillDeliveryAddress,
                    Phone = bill.BillDeliveryPhone,
                    DeliveryDate = bill.BillDeliveryDate,
                    Deposit = bill.BillDeposit,
                    Note = bill.BillNote,
                    ReceiveDate = bill.BillReceiveDate,
                    Status = bill.BillStatus,
                    Total = bill.BillTotal,
                    CakeContent = bill.BillCakeContent,
                    CakeName = cake?.CakeName,
                    CakeSize = cake?.CakeSize,
                    Acessory = acessory?.AcessoryName,
                    Quantity = bill.BillCakeQuantity,
                    BillShopId = bill.BillShopId,
                });
            }
            return Ok(response);
        }

        /// ✅ **Đặt hàng cho khách đã đăng nhập**
        [HttpPost("CreateBill")]
        public async Task<ActionResult> CreateBill(Bill bill)
        {
            // Lấy ID người dùng từ token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("Bạn cần đăng nhập để thực hiện thao tác này.");
            }

            // Gán ID người dùng vào đơn hàng
            bill.BillCustomId = userId; // Gắn ID người dùng vào Bill

            // Tạo ID mới cho đơn hàng
            bill.Id = ObjectId.GenerateNewId();

            // Xử lý các thông tin đơn hàng như trạng thái, tổng tiền, thời gian giao hàng
            bill.BillStatus = BillStatus.Pending; // Đơn mới luôn ở trạng thái "Chờ xử lý"
            bill.BillReceiveDate = DateTime.Now.AddDays(3); // Giao hàng sau 3 ngày

            // Tính toán tổng tiền đơn hàng
            var cake = await _cakeService.GetByIdAsync(bill.BillCakeId);
            if (cake == null)
            {
                return NotFound("Không tìm thấy sản phẩm!");
            }
            bill.BillShopId = cake.UserId;
            bill.BillTotal = bill.BillCakeQuantity * cake.CakePrice; // Tổng tiền đơn hàng

            // Thêm thông tin về nội dung bánh và ghi chú
            bill.BillCakeContent = bill.BillCakeContent; // Nội dung bánh
            bill.BillNote = bill.BillNote; // Ghi chú đơn hàng

            // Tạo đơn hàng
            await _billService.CreateAsync(bill);

            return Ok(new { message = "Đặt hàng thành công!", billId = bill.Id.ToString() });
        }



        /// ✅ **Đặt hàng cho khách vãng lai (không đăng nhập)**
        [HttpPost("CreateBillForGuest")]
        [AllowAnonymous] // ✅ Cho phép khách không đăng nhập sử dụng API này
        public async Task<IActionResult> CreateBillForGuest([FromBody] Bill bill)
        {
            if (bill == null)
                return BadRequest("Thông tin đơn hàng không hợp lệ!");

            // ✅ Gán ID mới để tránh lỗi trùng lặp
            bill.Id = ObjectId.GenerateNewId();
            bill.BillStatus = BillStatus.Pending; // Đơn mới luôn ở trạng thái "Chờ xử lý"
            bill.BillDeposit = 0; // Không đặt cọc trước
            bill.BillReceiveDate = DateTime.Now.AddDays(3); // Giao hàng sau 3 ngày

            // ✅ Tự động lấy `BillShopId` từ `BillCakeId`
            var cake = await _cakeService.GetByIdAsync(bill.BillCakeId);
            if (cake == null)
                return NotFound("Không tìm thấy sản phẩm!");

            bill.BillShopId = cake.UserId; // Gán ID của shop bán bánh vào `BillShopId`
            bill.BillTotal = bill.BillCakeQuantity * cake.CakePrice; // Tính tổng tiền (giả định mỗi bánh 500k)


            await _billService.CreateAsync(bill);
            return Ok(new { message = "Đơn hàng cho khách vãng lai đã được tạo thành công!", billId = bill.Id.ToString(), shopId = bill.BillShopId });
        }



        /// ✅ **Cập nhật trạng thái đơn hàng**
        [HttpPut("UpdateBillStatus/{billId}")]
        public async Task<IActionResult> UpdateBillStatus(string billId, [FromBody] string status)
        {
            if (!Enum.TryParse(status, out BillStatus billStatus))
                return BadRequest("Trạng thái không hợp lệ!");

            var result = await _billService.UpdateOrderStatusAsync(billId, billStatus);
            return result ? Ok("Cập nhật trạng thái thành công!") : BadRequest("Cập nhật thất bại!");
        }

        /// ✅ **Lấy đơn hàng theo ID khách hàng**
        [HttpGet("GetBillByCustomId/{id}")]
        public async Task<ActionResult> GetBillByCustomId(string id)
        {
            var bills = await _billService.GetBillByCustomId(id);
            if (bills == null || bills.Count == 0)
                return NotFound("Không tìm thấy đơn hàng!");

            var response = bills.Select(async bill =>
            {
                var user = await _userService.GetByIdAsync(bill.BillCustomId ?? "");
                var cake = await _cakeService.GetByIdAsync(bill.BillCakeId ?? "");
                var acessory = await _acessoryService.GetByIdAsync(bill.BillAcessoriesId ?? "");

                return new
                {
                    Id = bill.Id.ToString(),
                    CustomName = user?.FullName ?? "Khách vãng lai",
                    Address = bill.BillDeliveryAddress,
                    Phone = bill.BillDeliveryPhone,
                    DeliveryDate = bill.BillDeliveryDate,
                    Deposit = bill.BillDeposit,
                    Note = bill.BillNote,
                    ReceiveDate = bill.BillReceiveDate,
                    Status = bill.BillStatus,
                    Total = bill.BillTotal,
                    CakeContent = bill.BillCakeContent,
                    CakeName = cake?.CakeName,
                    CakeSize = cake?.CakeSize,
                    Acessory = acessory?.AcessoryName,
                    Quantity = bill.BillCakeQuantity,
                };
            }).Select(task => task.Result).ToList();

            return Ok(response);
        }

        /// ✅ **Xóa đơn hàng**
        [HttpDelete("DeleteBill/{id}")]
        public async Task<ActionResult> DeleteBill(string id)
        {
            await _billService.DeleteAsync(id);
            return Ok("Xóa đơn hàng thành công!");
        }
    }
}
