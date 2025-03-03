using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cakee.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AcessoryController : ControllerBase
    {
        private readonly IAcessoryService _acessoryService;
        public AcessoryController(IAcessoryService acessoryService)
        {
            _acessoryService = acessoryService;
        }

        [HttpGet("Get All Acessory")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Acessory>>> GetAcessory()
        {
            var acessories = await _acessoryService.GetAllAsync();
            var response = new List<object>(); // This will hold the formatted response
            //if acessory not null then show, if null then show message not found
            if (acessories == null)
            {
                return NotFound("Acessory not found");
            }
            foreach (var acessory in acessories)
            {
                response.Add(new
                {
                    Id = acessory.Id.ToString(),
                    AcessoryName = acessory.AcessoryName,
                    AcessoryPrice = acessory.AcessoryPrice,
                    AcessoryShopId = acessory.UserId
                });
            }
            return Ok(response);
        }

        [HttpGet("Get Acessory By Id")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAcessoryById(string id)
        {
            var acessory = await _acessoryService.GetByIdAsync(id);
            if (acessory == null)
            {
                return NotFound("Acessory not found");
            }
            var response = new
            {
                Id = acessory.Id.ToString(),
                AcessoryName = acessory.AcessoryName,
                AcessoryPrice = acessory.AcessoryPrice,
                AcessoryShopId = acessory.UserId
            };
            return Ok(response);

        }

        [HttpGet("Get Acessory By Name")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAcessoryByName(string acessoryName)
        {
            var acessory = await _acessoryService.GetByNameAsync(acessoryName);
            if (acessory == null)
            {
                return NotFound("Acessory not found");
            }
            var response = new
            {
                Id = acessory.Id.ToString(),
                AcessoryName = acessory.AcessoryName,
                AcessoryPrice = acessory.AcessoryPrice,
                AcessoryShopId = acessory.UserId
            };
            return Ok(response);
        }

        [HttpGet("GetAcessoryByUserId")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAcessoryByUserId(string userId)

        {
            var acessories = await _acessoryService.GetAcessoryByUserIdAsync(userId);
            if (acessories == null || acessories.Count == 0)
            {
                return NotFound(new { message = "No categories found." });
            }
            return Ok(acessories.Select(c => new
            {
                Id = c.Id.ToString(),
                AcessoryName = c.AcessoryName,
                AcessoryPrice = c.AcessoryPrice,
                UserId = c.UserId
            }));
        }


        [HttpPost("Create Acessory")]
        public async Task<ActionResult> CreateAcessory(Acessory acessory)
        {
            // Check if the acessory name exists
            var existingAcessory = await _acessoryService.GetByNameAsync(acessory.AcessoryName);
            if (existingAcessory != null)
            {
                return BadRequest("Acessory name already exists");
            }
            await _acessoryService.CreateAsync(acessory);
            return Ok(acessory);
        }

        [HttpPatch("Update Acessory")]
        public async Task<ActionResult> UpdateAcessory(string id, Acessory acessory)
        {
            // Check if the acessory name exists
            var existingAcessory = await _acessoryService.GetByNameAsync(acessory.AcessoryName);
            if (existingAcessory != null && existingAcessory.Id != acessory.Id)
            {
                return BadRequest("Acessory name already exists");
            }
            await _acessoryService.UpdateAsync(id, acessory);
            return Ok(acessory);
        }

        [HttpDelete("Delete Acessory")]
        public async Task<ActionResult> DeleteAcessory(string id)
        {
            var existingAcessory = await _acessoryService.GetByIdAsync(id);
            if (existingAcessory == null)
            {
                return NotFound("Acessory not found");
            }
            await _acessoryService.DeleteAsync(id);
            return Ok("Acessory deleted successfully");
        }
    }
}
