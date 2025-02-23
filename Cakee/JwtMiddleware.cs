using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cakee.Models;
using MongoDB.Bson;
using System.Security.Claims;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly IMongoCollection<User> _usersCollection;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration, IMongoDatabase database)
    {
        _next = next;
        _configuration = configuration;
        _usersCollection = database.GetCollection<User>("User");
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "").Trim();

        Console.WriteLine($"🟢 Token nhận được từ client: {token}"); // Debug token

        if (!string.IsNullOrEmpty(token))
        {
            await AttachUserToContext(context, token);
        }

        await _next(context);
    }


    private async Task AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            Console.WriteLine("🟢 Bắt đầu xác thực token...");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["AppSettings:SecretKey"]);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            Console.WriteLine($"✅ Token hợp lệ! UserID: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("❌ Không tìm thấy userId trong token!");
                return;
            }

            // Kiểm tra nếu ObjectId hợp lệ trước khi query MongoDB
            if (!ObjectId.TryParse(userId, out ObjectId objectId))
            {
                Console.WriteLine("❌ userId không hợp lệ!");
                return;
            }

            // Lấy thông tin user từ MongoDB
            var user = await _usersCollection.Find(u => u.Id == objectId).FirstOrDefaultAsync();
            if (user != null)
            {
                Console.WriteLine($"✅ Tìm thấy user: {user.UserName} trong database!");
                context.Items["User"] = user; // Gán user vào HttpContext.Items
            }
            else
            {
                Console.WriteLine("❌ Không tìm thấy user trong database!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi xác thực token: {ex.Message}");
        }
    }





}
