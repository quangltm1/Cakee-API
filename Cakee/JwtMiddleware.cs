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
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        var token = authHeader?.Split(' ').LastOrDefault(); // Lấy phần cuối cùng của chuỗi

        Console.WriteLine($"🟢 Token nhận được: {token}");

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

            // Debug: In toàn bộ claims từ token
            Console.WriteLine("🔍 Claims từ token:");
            foreach (var claim in jwtToken.Claims)
            {
                Console.WriteLine($"👉 {claim.Type}: {claim.Value}");
            }

            // Lấy userId từ các key phổ biến trong JWT
            var userId = jwtToken.Claims.FirstOrDefault(x =>
                x.Type == "nameid" || x.Type == "id" || x.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("❌ Không tìm thấy userId trong token!");
                return;
            }

            // Chuyển userId về ObjectId để truy vấn MongoDB
            if (!ObjectId.TryParse(userId, out ObjectId objectId))
            {
                Console.WriteLine($"❌ userId '{userId}' không phải ObjectId hợp lệ!");
                return;
            }

            var user = await _usersCollection.Find(u => u.Id == objectId).FirstOrDefaultAsync();
            if (user == null)
            {
                Console.WriteLine($"❌ Không tìm thấy user với ObjectId: {objectId}");
                return;
            }

            Console.WriteLine($"✅ User xác thực thành công: {user.UserName}");
            context.Items["User"] = user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi xác thực token: {ex.Message}");
        }
    }
}
