﻿using Microsoft.AspNetCore.Mvc;
using Cakee.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Cakee.Services.Service;
using Cakee.Services.IService;

namespace Cakee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bổ sung authorize cho toàn bộ controller
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly CakeService _cakeService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/<CategoryController>
        [HttpGet("Get All Category")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> GetCategory()
        {
            var categories = await _categoryService.GetAllAsync();
            var response = categories.Select(category => new
            {
                categoryId = category.Id.ToString(), // ✅ Đổi "id" thành "categoryId"
                categoryName = category.CategoryName,
                userId = category.UserId
            }).ToList();

            return Ok(response);
        }




        // GET api/<CategoryController>/5
        [HttpGet("Get Category By Id")]
        [AllowAnonymous] // Cho phép truy cập không cần xác thực
        public async Task<ActionResult> GetCategoryById(string id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound("Category not found");
            }

            var response = new
            {
                Id = category.Id.ToString(),  // Convert ObjectId to string
                CategoryName = category.CategoryName
            };

            return Ok(response); // Return the found category
        }

        // POST api/<CategoryController>
        [HttpPost("Create Category")]
        public async Task<ActionResult<Category>> Post([FromBody] Category category)
        {
            // Check if the category name already exists
            var existingCategory = await _categoryService.GetByNameAsync(category.CategoryName);
            if (existingCategory != null)
            {
                return BadRequest(new { message = "Category name already exists." });
            }

            var createdCategory = await _categoryService.CreateAsync(category);
            return CreatedAtAction("GetCategoryById", new { id = createdCategory.Id }, createdCategory);
        }

        //[HttpPut("Update Category")]
        //public async Task<ActionResult> Put(string id, [FromBody] Category updatedCategory)
        //{
        //    // Check if the category exists
        //    var existingCategory = await _categoryService.GetByIdAsync(id);
        //    if (existingCategory == null)
        //    {
        //        return NotFound(new { message = "Category not found." });
        //    }

        //    // Verify if the new category name already exists (excluding the current category)
        //    var categoryWithSameName = await _categoryService.GetByNameAsync(updatedCategory.CategoryName);
        //    if (categoryWithSameName != null && categoryWithSameName.Id != existingCategory.Id)
        //    {
        //        return BadRequest(new { message = "Category name already exists." });
        //    }

        //    // Update the category name
        //    existingCategory.CategoryName = updatedCategory.CategoryName;

        //    // Save changes
        //    await _categoryService.UpdateAsync(id, existingCategory);

        //    // Return a success message
        //    return Ok(new
        //    {
        //        message = "Category updated successfully.",
        //        category = new
        //        {
        //            Id = existingCategory.Id.ToString(),
        //            CategoryName = existingCategory.CategoryName
        //        }
        //    });
        //}

        // PATCH an existing category (partial update)
        [HttpPatch("Update Category")]
        public async Task<ActionResult> Patch(string id, [FromBody] Category updatedCategory)
        {
            // Check if the category exists
            var existingCategory = await _categoryService.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound(new { message = "Category not found." });
            }
            // Update the category name
            existingCategory.CategoryName = updatedCategory.CategoryName;
            // Save changes
            await _categoryService.UpdateAsync(id, existingCategory);
            // Return a success message
            return Ok(new
            {
                message = "Category updated successfully.",
                category = new
                {
                    Id = existingCategory.Id.ToString(),
                    CategoryName = existingCategory.CategoryName
                }
            });
        }

        [HttpGet("Get Category Name By Id")]
        [AllowAnonymous] // Cho phép truy cập không cần xác thực
        public async Task<IActionResult> GetByNameByIdAsync(string id)
        {
            var categoryName = await _categoryService.GetByNameByIdAsync(id); // Get category name by ID

            if (categoryName == null)
            {
                return NotFound("Category not found");
            }

            return Ok(new { CategoryName = categoryName }); // Return the category name
        }

        [HttpGet("GetCategoryByUserId")]
        public async Task<IActionResult> GetCategoryByUserId(string userId)

        {
            var categories = await _categoryService.GetCategoriesByUserIdAsync(userId);
            if (categories == null || categories.Count == 0)
            {
                return NotFound(new { message = "No categories found." });
            }
            return Ok(categories.Select(c => new
            {
                Id = c.Id.ToString(),
                CategoryName = c.CategoryName,
                UserId = c.UserId
            }));
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("Delete Category")]
        public async Task<ActionResult> Delete(string id)
        {
            // Check if the category exists
            var existingCategory = await _categoryService.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }

            // Delete the category
            await _categoryService.DeleteAsync(id);

            return Ok(new
            {
                message = "Category deleted successfully.",
                Id = id
            });
        }
    }
}
