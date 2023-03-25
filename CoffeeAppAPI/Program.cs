using CoffeeAppAPI.Services;

var builder = WebApplication.CreateBuilder(args);


// Add this line to register CosmosDbService
builder.Services.AddSingleton<CosmosDbService>();
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

app.Run();
