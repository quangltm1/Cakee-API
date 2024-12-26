using Microsoft.AspNetCore.Mvc;
using Cakee.Models;
using Cakee.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Cakee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/<CategoryController>
        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetCategory()
        {
            var categories = await _categoryService.GetAllAsync();
            var response = new List<object>(); // This will hold the formatted response

            foreach (var category in categories)
            {
                response.Add(new
                {
                    Id = category.Id.ToString(),  // Convert ObjectId to string
                    CategoryName = category.CategoryName
                });
            }

            return Ok(response);
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
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
        [HttpPost]
        public async Task<ActionResult<Category>> Post([FromBody] Category category)
        {
            var createdCategory = await _categoryService.CreateAsync(category);
            return CreatedAtAction("GetCategoryById", new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, [FromBody] Category updatedCategory)
        {
            // Check if the category exists
            var existingCategory = await _categoryService.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            // Verify if the new category name already exists (excluding the current category)
            var categoryWithSameName = await _categoryService.GetByNameAsync(updatedCategory.CategoryName);
            if (categoryWithSameName != null && categoryWithSameName.Id != existingCategory.Id)
            {
                return BadRequest(new { message = "Category name already exists." });
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




        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
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
