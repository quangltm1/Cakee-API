using Cakee.Models;
using Cakee.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cakee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CakeController : ControllerBase
    {
        private readonly ICakeService _cakeService;
        private readonly ICategoryService _cate;

        public CakeController(ICakeService cakeService)
        {
            _cakeService = cakeService;
        }
        // GET: api/<CakeController>
        [HttpGet("Get All Cake")]
        public async Task<ActionResult<List<Cake>>> GetCake()
        {
            var cakes = await _cakeService.GetAllAsync();
            var response = new List<object>(); // This will hold the formatted response
            //if cake not null then show, if null then show message not found
            if (cakes == null)
            {
                return NotFound("Cake not found");
            }
            foreach (var cake in cakes)
            {
                response.Add(new
                {
                    Id = cake.Id.ToString(),
                    CakeName = cake.CakeName,
                    CakeCategoryId = cake.CakeCategoryId.ToString(),
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
        public async Task<ActionResult> GetCakeById(string id)
        {
            var cake = await _cakeService.GetByIdAsync(id);
            if (cake == null)
            {
                return NotFound("Cake not found");
            }
            var response = new
            {
                //Id = cake.Id.ToString(),  // Convert ObjectId to string
                CakeName = cake.CakeName,
                CakeSize = cake.CakeSize,
                CakePrice = cake.CakePrice,
                CakeImage = cake.CakeImage,
                CakeCategory = cake.CakeCategoryId.ToString(), // Changed to CakeCategoryName
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

        // PUT api/<CakeController>/5
        [HttpPut("Update Cake")]
        public async Task<ActionResult> Put(string id, [FromBody] Cake updatedCake)
        {
            // Check if the cake exists
            var existingCake = await _cakeService.GetByIdAsync(id);
            if (existingCake == null)
            {
                return NotFound("Cake not found");
            }

            // Check if a cake with the updated name already exists (except for the current cake)
            var duplicateCake = await _cakeService.GetByNameAsync(updatedCake.CakeName);
            if (duplicateCake != null && duplicateCake.Id.ToString() != id)
            {
                return BadRequest(new { message = "Cake name already exists." });
            }

            await _cakeService.UpdateAsync(id, updatedCake);
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
