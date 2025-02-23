using System.Text;
using Cakee.Models;
using Cakee;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Cakee.Services.Service;
using Cakee.Services.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNgrok",
        builder =>
        {
            builder.WithOrigins("https://lobster-tops-pegasus.ngrok-free.app")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// MongoDb Connection
// MongoDb Connection
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("MyDb"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
builder.Services.AddSingleton<MongoClient>(sp =>
    new MongoClient(builder.Configuration.GetValue<string>("MyDb:ConnectionString"))
);
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<MongoClient>();
    var databaseSettings = sp.GetRequiredService<DatabaseSettings>();
    return client.GetDatabase(databaseSettings.DatabaseName);
});
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSettings"));


// Configure JWT Authentication
var secretKey = builder.Configuration["AppSettings:SecretKey"]
                ?? throw new ArgumentNullException("SecretKey không được để trống!");
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    context.Token = authHeader.Replace("Bearer ", "").Trim();
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.Response.Headers["WWW-Authenticate"] =
                    context.Response.Headers["WWW-Authenticate"].ToString().Replace("Bearer ", "");
                return Task.CompletedTask;
            }
        };
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidIssuer = "yourIssuer",
            ValidAudience = "yourAudience",
            ClockSkew = TimeSpan.Zero
        };
    });




// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cakee", Version = "v1" });
    c.SchemaFilter<CakeSchemaFilter>();
    c.SchemaFilter<CategorySchemaFilter>();
    c.SchemaFilter<UserSchemaFilter>();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token vào đây theo định dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer" // Quan trọng: Phải giữ nguyên "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer" // Quan trọng: Phải khớp với định nghĩa trên
            }
        },
        new List<string>()
    }
});
});

// Resolving the dependencies
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<ICakeService, CakeService>();
builder.Services.AddTransient<ICakeSizeService, CakeSizeService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IBillService, BillService>();
builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddTransient<IAcessoryService, AcessoryService>();
builder.Services.AddTransient<IStatisticalService, StatisticalService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowNgrok");

// Đăng ký middleware JWT
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

