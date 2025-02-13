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
        public BillController(IBillService billService)
        {
            _billService = billService;
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
                response.Add(new
                {
                    Id = bill.Id.ToString(),
                    BillFullName = bill.BillFullName,
                    BillName = bill.BillName,
                    BillPhone = bill.BillPhone,
                    BillAddress = bill.BillAddress,
                    BillDeliveryDate = bill.BillDeliveryDate,
                    BillReceiveDate = bill.BillReceiveDate,
                    BillDeposit = bill.BillDeposit,
                    BillTotal = bill.BillTotal,
                    BillStatus = bill.BillStatus,
                    BillNote = bill.BillNote,
                    BillContent = bill.BillContent,
                    UserId = bill.UserId.ToString(),
                    CakeIds = bill.CakeIds
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
            var response = new
            {
                Id = bill.Id.ToString(),
                BillFullName = bill.BillFullName,
                BillName = bill.BillName,
                BillPhone = bill.BillPhone,
                BillAddress = bill.BillAddress,
                BillDeliveryDate = bill.BillDeliveryDate,
                BillReceiveDate = bill.BillReceiveDate,
                BillDeposit = bill.BillDeposit,
                BillTotal = bill.BillTotal,
                BillStatus = bill.BillStatus,
                BillNote = bill.BillNote,
                BillContent = bill.BillContent,
                UserId = bill.UserId.ToString(),
                CakeIds = bill.CakeIds
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
            return Ok
            (
                new
                {
                    message = "Bill updated successfully",
                    bill = new
                    {
                        Id = bill.Id.ToString(),
                        BillFullName = bill.BillFullName,
                        BillName = bill.BillName,
                        BillPhone = bill.BillPhone,
                        BillAddress = bill.BillAddress,
                        BillDeliveryDate = bill.BillDeliveryDate,
                        BillReceiveDate = bill.BillReceiveDate,
                        BillDeposit = bill.BillDeposit,
                        BillTotal = bill.BillTotal,
                        BillStatus = bill.BillStatus,
                        BillNote = bill.BillNote,
                        BillContent = bill.BillContent,
                        UserId = bill.UserId.ToString(),
                        CakeIds = bill.CakeIds
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
