using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    // ICoffeeRepository.cs
    public interface ICoffeeRepository : IRepository<Coffee>
    {
        // Add any Coffee-specific methods here, if needed
    }

    // CoffeeRepository.cs
    public class CoffeeRepository : CosmosDbRepository<Coffee>, ICoffeeRepository
    {

        ICosmosDbService _cosmosDbService;
        public CoffeeRepository(ICosmosDbService cosmosDbService)
            : base(cosmosDbService, "Coffees", "/id", "Coffee")
        {
            _cosmosDbService = cosmosDbService;
        }

        // Implement any Coffee-specific methods here, if needed


        public async Task<Container> GetCoffeesContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("Coffees", "/id");
        }

        public async Task<IEnumerable<Coffee>> GetAllCoffeesAsync()
        {
            var coffeesContainer = await GetCoffeesContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<Coffee>(coffeesContainer);
        }

        public async Task<Coffee> GetCoffeeAsync(Guid id)
        {
            var coffeesContainer = await GetCoffeesContainerAsync();
            return await _cosmosDbService.GetItemAsync<Coffee>(coffeesContainer, id.ToString());
        }

        public async Task CreateCoffeeAsync(Coffee coffee)
        {
            var coffeesContainer = await GetCoffeesContainerAsync();
            await _cosmosDbService.AddItemAsync(coffeesContainer, coffee);
        }

        public async Task UpdateCoffeeAsync(Coffee coffee)
        {
            var coffeesContainer = await GetCoffeesContainerAsync();
            await _cosmosDbService.UpdateItemAsync(coffeesContainer, coffee.id.ToString(), coffee);
        }

        public async Task DeleteCoffeeAsync(Guid id)
        {
            var coffeesContainer = await GetCoffeesContainerAsync();
            await _cosmosDbService.DeleteItemAsync<Coffee>(coffeesContainer, id.ToString());
        }
    }
}
