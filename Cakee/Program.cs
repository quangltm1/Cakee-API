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
//resolving the CategoryService dependency here
builder.Services.AddTransient<ICategoryService, CategoryService>();
//resolving the CakeService dependency here
builder.Services.AddTransient<ICakeService, CakeService>();
//resolving the UserService dependency here
builder.Services.AddTransient<IUserService, UserService>();
//builder.Services.AddTransient<IProductService, ProductService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
