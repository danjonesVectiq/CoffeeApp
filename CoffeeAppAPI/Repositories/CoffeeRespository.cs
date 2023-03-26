using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class CoffeeRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public CoffeeRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

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
