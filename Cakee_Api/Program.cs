using Cakee_Api.Datas;
using Cakee_Api.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });;

// MongoDb Connection
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MyMongoDb")
);
builder.Services.AddSingleton<MongoDBSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoDBSettings>>().Value
);
builder.Services.AddSingleton<MongoClient>(sp =>
    new MongoClient(builder.Configuration.GetValue<string>("MyMongoDb:ConnectionString"))
);

// Register application services
builder.Services.AddTransient<ICakeService, CakeService>();
// Uncomment and register other services as needed
// builder.Services.AddTransient<ICategoryService, CategoryService>();
// builder.Services.AddTransient<IUserService, UserService>();
// builder.Services.AddTransient<IBillService, BillService>();
// builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
// builder.Services.AddTransient<IStatisticalService, StatisticalService>();
// builder.Services.AddTransient<ICakeSizeService, CakeSizeService>();
// builder.Services.AddTransient<IAcessoryService, AcessoryService>();

// Add logging
builder.Services.AddLogging();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
