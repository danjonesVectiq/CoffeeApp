using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class CoffeeListRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public CoffeeListRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetCoffeeListsContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("CoffeeLists", "/id");
        }

        public async Task<IEnumerable<CoffeeList>> GetAllCoffeeListsAsync()
        {
            var coffeeListsContainer = await GetCoffeeListsContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<CoffeeList>(coffeeListsContainer);
        }

        public async Task<CoffeeList> GetCoffeeListAsync(Guid id)
        {
            var coffeeListsContainer = await GetCoffeeListsContainerAsync();
            return await _cosmosDbService.GetItemAsync<CoffeeList>(coffeeListsContainer, id.ToString());
        }

        public async Task CreateCoffeeListAsync(CoffeeList coffeeList)
        {
            var coffeeListsContainer = await GetCoffeeListsContainerAsync();
            await _cosmosDbService.AddItemAsync(coffeeListsContainer, coffeeList);
        }

        public async Task UpdateCoffeeListAsync(CoffeeList coffeeList)
        {
            var coffeeListsContainer = await GetCoffeeListsContainerAsync();
            await _cosmosDbService.UpdateItemAsync(coffeeListsContainer, coffeeList.id.ToString(), coffeeList);
        }

        public async Task DeleteCoffeeListAsync(Guid id)
        {
            var coffeeListsContainer = await GetCoffeeListsContainerAsync();
            await _cosmosDbService.DeleteItemAsync<CoffeeList>(coffeeListsContainer, id.ToString());
        }
    }
}
