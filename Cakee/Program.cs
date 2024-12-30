using Cakee.Models;
using Cakee.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Resolving the CategoryService dependency here
builder.Services.AddTransient<ICategoryService, CategoryService>();
// Resolving the CakeService dependency here
builder.Services.AddTransient<ICakeService, CakeService>();
// Resolving the UserService dependency here
builder.Services.AddTransient<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Removed to disable HTTPS redirection

app.UseAuthorization();

app.MapControllers();

// Use the PORT environment variable provided by Heroku
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000"; // Default to 5000 if not set
app.Run($"http://0.0.0.0:{port}"); // Run on the appropriate port
