using CoffeeAppAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using CoffeeAppAPI.Data;

var builder = WebApplication.CreateBuilder(args);


// Add this line to register CosmosDbService
builder.Services.AddSingleton<SearchService>();
builder.Services.AddSingleton<IndexManagementService>();
builder.Services.AddSingleton<DataSeeder>();
builder.Services.AddSingleton<ICosmosDbService, CosmosDbService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
