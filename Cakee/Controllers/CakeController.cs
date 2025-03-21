﻿using Cakee.Models;
using Cakee.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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

        [HttpGet("text-search")]
        [AllowAnonymous]
        public async Task<IActionResult> TextSearch([FromQuery] string query)
        {
            var filter = Builders<Cake>.Filter.Text(query); // Use Cake instead of Product
            var products = await _cakeService.GetAllAsync(); // Fetch all cakes
            var filteredProducts = products.Where(c => c.CakeName.Contains(query, StringComparison.OrdinalIgnoreCase) || c.CakeDescription.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(filteredProducts);
        }

        [HttpGet("GetCakesByCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCakesByCategory([FromQuery] string? categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return BadRequest(new { message = "Thiếu ID danh mục." });
            }

            try
            {
                var cakes = await _cakeService.GetCakesByCategoryIdAsync(categoryId);
                if (cakes == null || cakes.Count == 0)
                {
                    return NotFound(new { message = "Không có bánh nào trong danh mục này." });
                }

                return Ok(cakes.Select(c => new
                {
                    Id = c.Id.ToString(),
                    c.CakeName,
                    c.CakeSize,
                    c.CakeDescription,
                    c.CakePrice,
                    c.CakeImage,
                    c.CakeRating,
                    c.CakeQuantity,
                    CakeCategoryId = c.CakeCategoryId,
                    UserId = c.UserId
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server.", error = ex.Message });
            }
        }





        // GET: api/<CakeController>
        [HttpGet("Get_All_Cake")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Cake>>> GetCakes()
        {
            var cakes = await _cakeService.GetAllAsync();
            return Ok(cakes.Select(c => new
            {
                Id = c.Id.ToString(),
                c.CakeName,
                c.CakeSize,
                c.CakeDescription,
                c.CakePrice,
                c.CakeImage,
                c.CakeRating,
                c.CakeQuantity,
                CakeCategoryId = c.CakeCategoryId,
                UserId = c.UserId
            }));
        }

        [HttpGet("GetAllCategories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _cate.GetAllAsync();

            if (categories == null || categories.Count == 0)
            {
                return NotFound(new { message = "Không có danh mục nào." });
            }

            return Ok(categories.Select(c => new
            {
                Id = c.Id.ToString(),
                Name = c.CategoryName,
                UserId = c.UserId
            }));
        }




        [HttpGet("Get Category Of Cake")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryOfCake(string cakeId)
        {
            if (!ObjectId.TryParse(cakeId, out ObjectId objectId))
            {
                return BadRequest(new { message = "ID bánh không hợp lệ." });
            }

            var category = await _cakeService.GetCategoryByCakeIdAsync(cakeId);

            if (category == null)
            {
                return NotFound(new { message = "Không tìm thấy danh mục." });
            }

            return Ok(new { CategoryName = category.CategoryName });
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
                Id = cake.Id.ToString(),  // Convert ObjectId to string
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

        [AllowAnonymous]
        [HttpGet("GetByUserId")]
        public async Task<IActionResult> GetCakesByStore([FromQuery] string userId)
        {
            if (!ObjectId.TryParse(userId, out ObjectId objectId))
            {
                return BadRequest(new { Message = "Invalid storeId format" });
            }

            var cakes = await _cakeService.GetCakesByUserIdAsync(userId);
            if (cakes == null || cakes.Count == 0)
            {
                return NotFound(new { Message = "No cakes found for this store" });
            }

            var response = cakes.Select(cake => new
            {
                Id = cake.Id.ToString(),  // ✅ Chuyển ObjectId sang string
                CakeName = cake.CakeName,
                CakeSize = cake.CakeSize,
                CakeDescription = cake.CakeDescription,
                CakePrice = cake.CakePrice,
                CakeImage = cake.CakeImage,
                CakeRating = cake.CakeRating,
                CakeQuantity = cake.CakeQuantity,
                CakeCategoryId = cake.CakeCategoryId, // ✅ Chuyển ObjectId sang string
                UserId = cake.UserId, // ✅ Chuyển ObjectId sang string
            });

            return Ok(response);
        }


        // POST api/<CakeController>
        [HttpPost("Create Cake")]
        public async Task<ActionResult<Cake>> Post([FromBody] Cake cake)
        {
            if (cake == null)
            {
                return BadRequest(new { message = "Cake data is required." });
            }

            // Kiểm tra tên bánh đã tồn tại chưa
            var existingCake = await _cakeService.GetByNameAsync(cake.CakeName);
            if (existingCake != null)
            {
                return BadRequest(new { message = "Cake name already exists." });
            }

            // ✅ Không cần ObjectId.Parse()
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

        // Cập nhật bánh
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCake(string id, [FromBody] UpdateCakeRequest request)
        {
            var existingCake = await _cakeService.GetByIdAsync(id);
            if (existingCake == null)
                return NotFound(new { message = "Cake not found." });

            existingCake.CakeName = request.CakeName ?? existingCake.CakeName;
            existingCake.CakeDescription = request.CakeDescription ?? existingCake.CakeDescription;
            existingCake.CakePrice = request.CakePrice ?? existingCake.CakePrice;
            existingCake.CakeImage = request.CakeImage ?? existingCake.CakeImage;
            existingCake.CakeQuantity = request.CakeQuantity ?? existingCake.CakeQuantity;

            if (!string.IsNullOrEmpty(request.CakeCategoryId))
            {
                existingCake.CakeCategoryId = request.CakeCategoryId;  // ✅ Không cần chuyển đổi
            }

            await _cakeService.UpdateAsync(id, existingCake);
            return Ok(new { message = "Cake updated successfully." });
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
