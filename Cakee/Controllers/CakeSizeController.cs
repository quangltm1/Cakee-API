using Cakee.Models;
using Cakee.Services.IService;
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
        public CakeSizeController(ICakeSizeService cakeSizeService)
        {
            _cakeSizeService = cakeSizeService;
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
            };
            return Ok(response);
        }

        [HttpPost("Create Cake Size")]
        public async Task<ActionResult> CreateCakeSize(CakeSize cakesize)
        {
            await _cakeSizeService.CreateAsync(cakesize);
            return Ok("Cake Size created successfully");
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
                        CakeSizeName = existingCakeSize.SizeName
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
            // Delete the cake size
            await _cakeSizeService.DeleteAsync(id);
            // Return a success message
            return Ok(new { message = "Cake Size deleted successfully." });
        }
    }
}
