using Cakee.Models;
using Cakee.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace Cakee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("Get All Account")]
        public async Task<ActionResult> GetUser()
        {
            var users = await _userService.GetAllAsync();

            if (users == null || !users.Any())
            {
                return NotFound(new { Message = "Users not found" });
            }

            var response = users.Select(user => new
            {
                Id = user.Id.ToString(),
                userName = user.UserName,
                pass = user.PassWord,
                FullName = user.FullName,
                Phone = user.Phone,
                Role = user.Role
            });

            return Ok(response);
        }

        [HttpGet("Get Role Of User")]
        public async Task<ActionResult> GetUserByRole(int role)
        {
            var users = await _userService.GetByRoleAsync(role);

            if (users == null || !users.Any())
            {
                return NotFound(new { Message = $"No users found with role = {role}" });
            }

            return Ok(users);
        }

        // CREATE a new user
        [HttpPost("Create User")]
        public async Task<ActionResult> CreateUser([FromBody] User userDto)
        {
            if (userDto == null)
            {
                return BadRequest(new { Message = "User data is invalid" });
            }

            var user = new User
            {
                UserName = userDto.UserName,
                PassWord = userDto.PassWord,
                FullName = userDto.FullName,
                Phone = userDto.Phone,
            };
            userDto.Role = 0; // User

            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        //Create a new admin
        [HttpPost("Create Admin")]
        public async Task<ActionResult> CreateAdmin([FromBody] User userDto)
        {
            if (userDto == null)
            {
                return BadRequest(new { Message = "User data is invalid" });
            }

            var user = new User
            {
                UserName = userDto.UserName,
                PassWord = userDto.PassWord,
                FullName = userDto.FullName,
                Phone = userDto.Phone,
            };
            userDto.Role = 1; // Admin

            var createdUser = await _userService.CreateAdminAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        // UPDATE an existing user
        [HttpPut("Update Account")]
        public async Task<ActionResult> UpdateUser(string id, [FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(new { Message = "User data is invalid" });
            }

            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found" });
            }

            await _userService.UpdateAsync(id, user);
            return Ok(new { Message = "User updated successfully" });
        }

        // PATCH an existing user (partial update)
        [HttpPatch("Update User")]
        public async Task<ActionResult> UpdateUserPartial(string id, [FromBody] UserPatchRequest patchRequest)
        {
            if (patchRequest == null)
            {
                return BadRequest(new { Message = "Invalid update data" });
            }

            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found" });
            }

            // Update the specific fields
            if (!string.IsNullOrEmpty(patchRequest.Password))
            {
                existingUser.PassWord = patchRequest.Password;
            }

            if (!string.IsNullOrEmpty(patchRequest.Phone))
            {
                existingUser.Phone = patchRequest.Phone;
            }

            if (!string.IsNullOrEmpty(patchRequest.FullName))
            {
                existingUser.FullName = patchRequest.FullName;
            }

            // Update the user in the database
            await _userService.UpdateAsync(id, existingUser);

            return Ok(new { Message = "User updated successfully" });
        }




        // DELETE an existing user
        [HttpDelete("Delete Account")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found" });
            }

            await _userService.DeleteAsync(id);
            return Ok(new { Message = "User deleted successfully", Id = id });
        }
    }

}

