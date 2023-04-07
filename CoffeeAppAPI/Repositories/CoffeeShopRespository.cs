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

        public async Task<Container> GetCoffeeShopContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("Coffee", "/id");
        }

        public async Task<IEnumerable<CoffeeShop>> GetAllCoffeeShopsAsync()
        {
            var coffeeShopContainer = await GetCoffeeShopContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<CoffeeShop>(coffeeShopContainer, "CoffeeShop");
        }

        public async Task<CoffeeShop> GetCoffeeShopAsync(Guid id)
        {
            var coffeeShopsContainer = await GetCoffeeShopContainerAsync();
            return await _cosmosDbService.GetItemAsync<CoffeeShop>(coffeeShopsContainer, id.ToString());
        }

        public async Task CreateCoffeeShopAsync(CoffeeShop coffeeShop)
        {
            var coffeeShopsContainer = await GetCoffeeShopContainerAsync();
            await _cosmosDbService.AddItemAsync(coffeeShopsContainer, coffeeShop);
        }

        public async Task UpdateCoffeeShopAsync(CoffeeShop coffeeShop)
        {
            var coffeeShopsContainer = await GetCoffeeShopContainerAsync();
            await _cosmosDbService.UpdateItemAsync(coffeeShopsContainer, coffeeShop.id.ToString(), coffeeShop);
        }

        public async Task DeleteCoffeeShopAsync(Guid id)
        {
            var coffeeShopsContainer = await GetCoffeeShopContainerAsync();
            await _cosmosDbService.DeleteItemAsync<CoffeeShop>(coffeeShopsContainer, id.ToString());
        }
    }
}
