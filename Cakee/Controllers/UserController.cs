using Cakee.Models;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Cakee.Services.IService;

namespace Cakee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly AppSetting _appSetting;

        public UserController(IUserService userService, IConfiguration configuration, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _userService = userService;
            _configuration = configuration;
            _appSetting = optionsMonitor.CurrentValue;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.PassWord))
            {
                return BadRequest(new { Message = "Username và password là bắt buộc" });
            }

            var user = await _userService.GetUserByUserNameAsync(loginRequest.UserName);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return Unauthorized(new { Message = "Username hoặc password không hợp lệ" });
            }

            // Bỏ qua kiểm tra băm mật khẩu
            if (loginRequest.PassWord != user.PassWord)
            {
                Console.WriteLine("Password verification failed");
                return Unauthorized(new { Message = "Username hoặc password không hợp lệ" });
            }

            // Kiểm tra vai trò của người dùng (0 - user, 1 - admin)
            if (user.Role != 0 && user.Role != 1)
            {
                return Unauthorized(new { Message = "Vai trò không hợp lệ" });
            }

            // Tạo mã token (JWT)
            var token = await GenerateJwtToken(user);
            return Ok(new { Token = token, Role = user.Role });
        }

        private async Task<Token> GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_appSetting.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                            new Claim(JwtRegisteredClaimNames.PhoneNumber, user.Phone),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim("Username", user.UserName),
                            new Claim(ClaimTypes.Role, user.Role.ToString()),


                            new Claim("TokenId", Guid.NewGuid().ToString())
                        }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //var accessToken = tokenHandler.WriteToken(token);
            //var refreshToken = GenerateRefreshToken();

            //var refreshTokenModel = new RefreshToken
            //{
            //    Id = MongoDB.Bson.ObjectId.GenerateNewId(),
            //    UserId = user.Id,
            //    JwtId = token.Id,
            //    Token = refreshToken,
            //    IsUsed = false,
            //    IsRevoked = false,
            //    IssuedAt = DateTime.UtcNow,
            //    ExpiredAt = DateTime.UtcNow.AddHours(24)
            //};

            // Lưu refresh token vào database
            //await _userService.SaveRefreshTokenAsync(refreshTokenModel);

            return new Token
            {
                AccessToken = tokenHandler.WriteToken(token),
                //RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        //[HttpPost("RenewToken")]
        //public async Task<IActionResult> RenewToken(Token model)
        //{
        //    var jwtTokenHandler = new JwtSecurityTokenHandler();
        //    var secretKeyBytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);
        //    var tokenValidateParam = new TokenValidationParameters
        //    {
        //        //tự cấp token
        //        ValidateIssuer = false,
        //        ValidateAudience = false,

        //        //ký vào token
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

        //        ClockSkew = TimeSpan.Zero,

        //        ValidateLifetime = false //ko kiểm tra token hết hạn
        //    };
        //    try
        //    {
        //        //check 1: AccessToken valid format
        //        var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidateParam, out var validatedToken);

        //        //check 2: Check alg
        //        if (validatedToken is JwtSecurityToken jwtSecurityToken)
        //        {
        //            var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
        //            if (!result)//false
        //            {
        //                return Ok(new ApiResponse
        //                {
        //                    Success = false,
        //                    Message = "Invalid token"
        //                });
        //            }
        //        }

        //        //check 3: Check accessToken expire?
        //        var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

        //        var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
        //        if (expireDate > DateTime.UtcNow)
        //        {
        //            return Ok(new ApiResponse
        //            {
        //                Success = false,
        //                Message = "Access token has not yet expired"
        //            });
        //        }

        //        //check 4: Check refreshtoken exist in DB
        //        var storedToken = await _userService.GetRefreshTokenAsync(model.RefreshToken);
        //        if (storedToken == null)
        //        {
        //            return Ok(new ApiResponse
        //            {
        //                Success = false,
        //                Message = "Refresh token does not exist"
        //            });
        //        }

        //        //check 5: check refreshToken is used/revoked?
        //        if (storedToken.IsUsed)
        //        {
        //            return Ok(new ApiResponse
        //            {
        //                Success = false,
        //                Message = "Refresh token has been used"
        //            });
        //        }
        //        if (storedToken.IsRevoked)
        //        {
        //            return Ok(new ApiResponse
        //            {
        //                Success = false,
        //                Message = "Refresh token has been revoked"
        //            });
        //        }

        //        //check 6: AccessToken id == JwtId in RefreshToken
        //        var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        //        if (storedToken.JwtId != jti)
        //        {
        //            return Ok(new ApiResponse
        //            {
        //                Success = false,
        //                Message = "Token doesn't match"
        //            });
        //        }

        //        //Update token is used
        //        storedToken.IsRevoked = true;
        //        storedToken.IsUsed = true;
        //        await _userService.UpdateRefreshTokenAsync(storedToken);

        //        //create new token
        //        var user = await _userService.GetByIdAsync(storedToken.UserId.ToString());
        //        var token = await GenerateJwtToken(user);

        //        return Ok(new ApiResponse
        //        {
        //            Success = true,
        //            Message = "Renew token success",
        //            Data = token
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ApiResponse
        //        {
        //            Success = false,
        //            Message = "Something went wrong"
        //        });
        //    }
        //}

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval = dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
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

        [HttpGet("Get User By Role")]
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
                PassWord = userDto.PassWord, // Không băm mật khẩu
                FullName = userDto.FullName,
                Phone = userDto.Phone,
                Role = 0 // User
            };

            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        // Create a new admin
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
                PassWord = userDto.PassWord, // Không băm mật khẩu
                FullName = userDto.FullName,
                Phone = userDto.Phone,
                Role = 1 // Admin
            };

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
                existingUser.PassWord = patchRequest.Password; // Không băm mật khẩu
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
