using Cakee.Models;
using Cakee.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUser()
        {
            var users = await _userService.GetAllAsync();
            var response = new List<object>(); // This will hold the formatted response
            //if user not null then show, if null then show message not found
            if (users == null)
            {
                return NotFound("User not found");
            }
            foreach (var user in users)
            {
                response.Add(new
                {
                    Id = user.Id.ToString(),
                    UserName = user.UserName,
                    UserPassword = user.PassWord,
                    UserFullName = user.FullName,
                    UserPhone = user.Phone,
                    UserRole = user.Role
                });
            }
            return Ok(response);
        }
    }
}
