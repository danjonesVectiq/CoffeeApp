using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class CoffeeShopRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public CoffeeShopRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetCoffeeShopsContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("Coffee", "/id");
        }

        public async Task<IEnumerable<Coffee>> GetAllCoffeesAsync()
        {
            var coffeeShopsContainer = await GetCoffeeShopsContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<Coffee>(coffeeShopsContainer, "CoffeeShop");
        }

        public async Task<Coffee> GetCoffeeAsync(Guid id)
        {
            var coffeeShopsContainer = await GetCoffeeShopsContainerAsync();
            return await _cosmosDbService.GetItemAsync<Coffee>(coffeeShopsContainer, id.ToString());
        }

        public async Task CreateCoffeeShopAsync(CoffeeShop coffee)
        {
            var coffeeShopsContainer = await GetCoffeeShopsContainerAsync();
            await _cosmosDbService.AddItemAsync(coffeeShopsContainer, coffee);
        }

        public async Task UpdateCoffeeAsync(Coffee coffee)
        {
            var coffeeShopsContainer = await GetCoffeeShopsContainerAsync();
            await _cosmosDbService.UpdateItemAsync(coffeeShopsContainer, coffee.id.ToString(), coffee);
        }

        public async Task DeleteCoffeeAsync(Guid id)
        {
            var coffeeShopsContainer = await GetCoffeeShopsContainerAsync();
            await _cosmosDbService.DeleteItemAsync<Coffee>(coffeeShopsContainer, id.ToString());
        }
    }
}
