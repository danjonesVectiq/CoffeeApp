using CoffeeAppAPI.Repositories;
using CoffeeAppAPI.Data;
using CoffeeAppAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

var AllowAll = "_AllowAll";

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowAll,
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                      });
});

// Register services
builder.Services.AddSingleton<SearchRepository>();
builder.Services.AddSingleton<IndexManagementRepository>();
builder.Services.AddSingleton<DataSeeder>();
builder.Services.AddSingleton<ICosmosDbRepository, CosmosDbRepository>();
builder.Services.AddSingleton<IBlobStorageRepository, BlobStorageRepository>();

builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<ISearchService, SearchService>();

builder.Services.AddScoped<ICoffeeShopService, CoffeeShopService>();

builder.Services.AddScoped<ICoffeeService, CoffeeService>();
builder.Services.AddScoped<IRoasterService, RoasterService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<ICheckInService, CheckInService>();
builder.Services.AddSingleton<ICoffeeScoringService, CoffeeScoringService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();



// Add controllers and Swagger
builder.Services.AddControllers();
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
app.UseCors(AllowAll);
app.UseAuthorization();

app.MapControllers();

// Check for the '--seed' argument
if (args.Contains("--seed"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await dataSeeder.CleanUpData(); // Clean up existing data
        await dataSeeder.SeedData(); // Seed new data
    }
}

app.Run();
