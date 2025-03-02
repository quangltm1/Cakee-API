using Cakee.Models;
using Cakee.Services.IService;
using Cakee.Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cakee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CakeSizeController : ControllerBase
    {
        private readonly ICakeSizeService _cakeSizeService;
        private readonly ICakeService _cakeService; 

        public CakeSizeController(ICakeSizeService cakeSizeService, ICakeService cakeService)
        {
            _cakeSizeService = cakeSizeService;
            _cakeService = cakeService;
        }

        [HttpGet("Get All Cake Size")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CakeSize>>> GetCakeSize()
        {
            var cakesizes = await _cakeSizeService.GetAllAsync();
            var response = new List<object>(); // This will hold the formatted response
            //if cake not null then show, if null then show message not found
            if (cakesizes == null)
            {
                return NotFound("Cake Size not found");
            }
            foreach (var cakesize in cakesizes)
            {
                response.Add(new
                {
                    Id = cakesize.Id.ToString(),
                    CakeSizeName = cakesize.SizeName,
                    ShopId = cakesize.UserId
                });
            }
            return Ok(response);
        }

        [HttpGet("Get Cake Size By Id")]
        [AllowAnonymous]
        public async Task<ActionResult> GetCakeSizeById(string id)
        {
            var cakesize = await _cakeSizeService.GetByIdAsync(id);
            if (cakesize == null)
            {
                return NotFound("Cake Size not found");
            }
            var response = new
            {
                Id = cakesize.Id.ToString(),
                CakeSizeName = cakesize.SizeName,
                ShopId = cakesize.UserId
            };
            return Ok(response);
        }

        [HttpPost("Create Cake Size")]
        public async Task<ActionResult> CreateCakeSize(CakeSize cakesize)
        {
            // Check if the size name exists
            var existingCakeSize = await _cakeSizeService.GetByNameAsync(cakesize.SizeName);
            if (existingCakeSize != null)
            {
                return BadRequest(new { message = "Size already exists, enter another size." });
            }
            // Create the cake size
            var newCakeSize = await _cakeSizeService.CreateAsync(cakesize);
            // Return a success message
            return CreatedAtAction(nameof(GetCakeSizeById), new { id = newCakeSize.Id.ToString() }, new
            {
                message = "Cake Size created successfully.",
                cakesize = new
                {
                    Id = newCakeSize.Id.ToString(),
                    CakeSizeName = newCakeSize.SizeName,
                    ShopId = cakesize.UserId
                }
            });
        }


        [HttpPatch("Update Cake Size")]
        public async Task<ActionResult> UpdateCakeSize(string id, [FromBody] CakeSize request)
        {
            // Check if the cake size exists
            var existingCakeSize = await _cakeSizeService.GetByIdAsync(id);
            if (existingCakeSize == null)
            {
                return NotFound(new { message = "Cake Size not found." });
            }
            // Update the cake size name
            existingCakeSize.SizeName = request.SizeName;
            // Save changes
            await _cakeSizeService.UpdateAsync(id, existingCakeSize);
            // Return a success message
            return Ok(
                new
                {
                    message = "Cake Size updated successfully.",
                    cakesize = new
                    {
                        Id = existingCakeSize.Id.ToString(),
                        CakeSizeName = existingCakeSize.SizeName,
                        ShopId = existingCakeSize.UserId
                    }
                });
        }


        [HttpDelete("Delete Cake Size")]
        public async Task<ActionResult> DeleteCakeSize(string id)
        {
            // Check if the cake size exists
            var existingCakeSize = await _cakeSizeService.GetByIdAsync(id);
            if (existingCakeSize == null)
            {
                return NotFound(new { message = "Cake Size not found." });
            }
            // Check if cake size is used in any cake
            var cake = await _cakeService.GetBySizeAsync(existingCakeSize.SizeName);
            if (cake != null)
            {
                return BadRequest(new { message = "Cake Size is used in a cake, cannot delete." });
            }
            // Delete the cake size
            await _cakeSizeService.DeleteAsync(id);
            // Return a success message
            return Ok(new { message = "Cake Size deleted successfully." });
        }
    }
}
