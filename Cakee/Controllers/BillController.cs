using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet("Get All Bill")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Bill>>> GetBill()
        {
            var bills = await _billService.GetAllAsync();
            var response = new List<object>(); // This will hold the formatted response
            //if bill not null then show, if null then show message not found
            if (bills == null)
            {
                return NotFound("Bill not found");
            }
            foreach (var bill in bills)
            {
                var user = await _userService.GetByIdAsync(bill.BillCustomId.ToString());
                var cake = await _cakeService.GetByIdAsync(bill.BillCakeId.ToString());
                var acessory = await _acessoryService.GetByIdAsync(bill.BillAcessoriesId);
                response.Add(new
                {
                    Id = bill.Id.ToString(),
                    CustomName = user?.FullName,
                    Address = bill.BillDeliveryAddress,
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
                });
            }
            return Ok(response);
        }
        [HttpGet("Get Bill By Id")]
        [AllowAnonymous]
        public async Task<ActionResult> GetBillById(string id)
        {
            var bill = await _billService.GetByIdAsync(id);
            if (bill == null)
            {
                return NotFound("Bill not found");
            }
            var user = await _userService.GetByIdAsync(bill.BillCustomId.ToString());
            var cake = await _cakeService.GetByIdAsync(bill.BillCakeId.ToString());
            var acessory = await _acessoryService.GetByIdAsync(bill.BillAcessoriesId);
            var response = new
            {
                Id = bill.Id.ToString(),
                CustomName = user?.FullName,
                Address = bill.BillDeliveryAddress,
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
            return Ok(response);
        }
        [HttpPost("Create Bill")]
        public async Task<ActionResult> CreateBill(Bill bill)
        {
            await _billService.CreateAsync(bill);
            return Ok("Bill created successfully");
        }

        [HttpPatch("Update Bill")]
        public async Task<ActionResult> UpdateBill(string id, [FromBody] Bill bill)
        {
            //check if the bill exist
            var existingBill = await _billService.GetByIdAsync(id);
            if (existingBill == null)
            {
                return NotFound(new { message = "Bill not found." });
            }
            //update the bill
            var billId = existingBill.Id;
            bill.Id = billId;
            await _billService.UpdateAsync(id, bill);
            var user = await _userService.GetByIdAsync(bill.BillCustomId.ToString());
            var cake = await _cakeService.GetByIdAsync(bill.BillCakeId.ToString());
            var acessory = await _acessoryService.GetByIdAsync(bill.BillAcessoriesId);
            return Ok
            (
                new
                {
                    message = "Bill updated successfully",

                    bill = new
                    {
                        Id = bill.Id.ToString(),
                        CustomName = user?.FullName,
                        Address = bill.BillDeliveryAddress,
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
                    }
                }
            );
        }

        [HttpDelete("Delete Bill")]
        public async Task<ActionResult> DeleteBill(string id)
        {
            await _billService.DeleteAsync(id);
            return Ok("Bill deleted successfully");
        }
    }
}
