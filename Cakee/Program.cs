﻿using System.Text;
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
using Cakee.Services.IService; // Added missing using directive

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// MongoDb Connection
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("MyDb")
);
builder.Services.AddSingleton<DatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<DatabaseSettings>>().Value
);
builder.Services.AddSingleton<MongoClient>(sp =>
    new MongoClient(builder.Configuration.GetValue<string>("MyDb:ConnectionString"))
);
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSettings")); // Fixed Configuration reference

// Configure JWT Authentication
var secretKey = builder.Configuration["AppSettings:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.Response.Headers["WWW-Authenticate"] = context.Response.Headers["WWW-Authenticate"].ToString().Replace("Bearer ", "");
                return Task.CompletedTask;
            }
        };
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
            ClockSkew = TimeSpan.Zero
        };
    });


// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"{token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
    c.SchemaFilter<UserSchemaFilter>();
    c.SchemaFilter<CakeSchemaFilter>();
    c.SchemaFilter<CategorySchemaFilter>();
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
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=User}/{action=Index}/{id?}");
});


app.Run();
