using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Cakee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bổ sung authorize cho toàn bộ controller
    public class CakeController : ControllerBase
    {
        private readonly ICakeService _cakeService;
        private readonly ICategoryService _cate;

        public CakeController(ICakeService cakeService, ICategoryService cate)
        {
            _cakeService = cakeService;
            _cate = cate;
        }

        // GET: api/<CakeController>
        [HttpGet("Get All Cake")]
        [AllowAnonymous] // Cho phép truy cập không cần xác thực
        public async Task<ActionResult<List<object>>> GetCake()
        {
            var cakes = await _cakeService.GetAllAsync();
            var response = new List<object>(); // This will hold the formatted response

            if (cakes == null)
            {
                return NotFound("Cake not found");
            }

            foreach (var cake in cakes)
            {
                var category = await _cate.GetByIdAsync(cake.CakeCategoryId.ToString());
                response.Add(new
                {
                    Id = cake.Id.ToString(),
                    CakeName = cake.CakeName,
                    CakeCategoryName = category?.CategoryName, // Display category name
                    CakeSize = cake.CakeSize,
                    CakeDescription = cake.CakeDescription,
                    CakePrice = cake.CakePrice,
                    CakeImage = cake.CakeImage,
                    CakeRating = cake.CakeRating,
                    CakeQuantity = cake.CakeQuantity,
                });
            }
            return Ok(response);
        }

        [HttpGet("Get Category Of Cake")]
        [AllowAnonymous] // Cho phép truy cập không cần xác thực
        public async Task<IActionResult> GetCategoryOfCake(string cakeId)
        {
            var cakes = await _cakeService.GetCategoryByCakeIdAsync(cakeId);
            var response = new
            {
                CategoryName = cakes.CategoryName.ToString(),
            };

            return Ok(response);
        }

        [HttpGet("Get Cake By Id")]
        [AllowAnonymous] // Cho phép truy cập không cần xác thực
        public async Task<ActionResult> GetCakeById(string id)
        {
            var cake = await _cakeService.GetByIdAsync(id);
            if (cake == null)
            {
                return NotFound("Cake not found");
            }
            var category = await _cate.GetByIdAsync(cake.CakeCategoryId.ToString());
            var response = new
            {
                //Id = cake.Id.ToString(),  // Convert ObjectId to string
                CakeName = cake.CakeName,
                CakeSize = cake.CakeSize,
                CakePrice = cake.CakePrice,
                CakeImage = cake.CakeImage,
                CategoryName = category?.CategoryName,// Changed to CakeCategoryName
                CakeRating = cake.CakeRating,
                CakeQuantity = cake.CakeQuantity
            };
            return Ok(response); // Return the found cake
        }

        // POST api/<CakeController>
        [HttpPost("Create Cake")]
        public async Task<ActionResult<Cake>> Post([FromBody] Cake cake)
        {
            // Check if a cake with the same name already exists
            var existingCake = await _cakeService.GetByNameAsync(cake.CakeName);
            if (existingCake != null)
            {
                return BadRequest(new { message = "Cake name already exists." });
            }

            var createdCake = await _cakeService.CreateAsync(cake);
            return CreatedAtAction("GetCakeById", new { id = createdCake.Id }, createdCake);
        }

        //// PUT api/<CakeController>/5
        //[HttpPut("Update Cake")]
        //public async Task<ActionResult> Put(string id, [FromBody] Cake updatedCake)
        //{
        //    // Check if the cake exists
        //    var existingCake = await _cakeService.GetByIdAsync(id);
        //    if (existingCake == null)
        //    {
        //        return NotFound("Cake not found");
        //    }

        //    // Check if a cake with the updated name already exists (except for the current cake)
        //    var duplicateCake = await _cakeService.GetByNameAsync(updatedCake.CakeName);
        //    if (duplicateCake != null && duplicateCake.Id.ToString() != id)
        //    {
        //        return BadRequest(new { message = "Cake name already exists." });
        //    }

        //    await _cakeService.UpdateAsync(id, updatedCake);
        //    return Ok("Cake updated successfully");
        //}

        // PATCH update cake (partial update)
        [HttpPatch("Update Cake")]
        public async Task<ActionResult> Patch(string id, [FromBody] Cake updatedCake)
        {
            // Check if the cake exists
            var existingCake = await _cakeService.GetByIdAsync(id);
            if (existingCake == null)
            {
                return NotFound("Cake not found");
            }
            // Update the specific fields
            if (updatedCake.CakeName != null)
            {
                existingCake.CakeName = updatedCake.CakeName;
            }
            if (updatedCake.CakeSize != null)
            {
                existingCake.CakeSize = updatedCake.CakeSize;
            }
            if (updatedCake.CakePrice != null)
            {
                existingCake.CakePrice = updatedCake.CakePrice;
            }
            if (updatedCake.CakeImage != null)
            {
                existingCake.CakeImage = updatedCake.CakeImage;
            }
            if (updatedCake.CakeDescription != null)
            {
                existingCake.CakeDescription = updatedCake.CakeDescription;
            }
            if (updatedCake.CakeQuantity != null)
            {
                existingCake.CakeQuantity = updatedCake.CakeQuantity;
            }
            await _cakeService.UpdateAsync(id, existingCake);
            return Ok("Cake updated successfully");
        }

        // DELETE api/<CakeController>/5
        [HttpDelete("Delete Cake")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingCake = await _cakeService.GetByIdAsync(id);
            if (existingCake == null)
            {
                return NotFound("Cake not found");
            }
            await _cakeService.DeleteAsync(id);
            return Ok(new
            {
                message = "Cake deleted successfully.",
                Id = id
            });
        }
    }
}
