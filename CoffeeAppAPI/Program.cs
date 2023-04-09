using CoffeeAppAPI.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using CoffeeAppAPI.Data;
using CoffeeAppAPI.Services;
var  AllowAll = "_AllowAll";

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddSingleton<SearchRepository>();
builder.Services.AddSingleton<IndexManagementRepository>();
builder.Services.AddSingleton<DataSeeder>();
builder.Services.AddSingleton<ICosmosDbRepository, CosmosDbRepository>();
builder.Services.AddSingleton<IBlobStorageRepository, BlobStorageRepository>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<ISearchService, SearchService>(); 
builder.Services.AddScoped<ICoffeeService, CoffeeService>();
builder.Services.AddScoped<ICoffeeShopService, CoffeeShopService>();
builder.Services.AddScoped<IRoasterService, RoasterService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<ICheckInService, CheckInService>();
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
