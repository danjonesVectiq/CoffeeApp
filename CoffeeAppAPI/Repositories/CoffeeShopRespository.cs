using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface ICoffeeShopRepository : IRepository<CoffeeShop>
    {
        // Add any CoffeeShop-specific methods here, if needed
    }

    public class CoffeeShopRepository : CosmosDbRepository<CoffeeShop>, ICoffeeShopRepository
    {
        public CoffeeShopRepository(ICosmosDbService cosmosDbService)
            : base(cosmosDbService, "Coffee", "/id", "CoffeeShop")
        {
        }

        // Implement any CoffeeShop-specific methods here, if needed

        public async Task<IEnumerable<CoffeeShop>> GetAllCoffeeShopsAsync()
        {
            return await GetAllAsync();
        }

        public async Task<CoffeeShop> GetCoffeeShopAsync(Guid id)
        {
            return await GetAsync(id);
        }

        public async Task CreateCoffeeShopAsync(CoffeeShop coffeeShop)
        {
            await CreateAsync(coffeeShop);
        }

        public async Task UpdateCoffeeShopAsync(CoffeeShop coffeeShop)
        {
            await UpdateAsync(coffeeShop);
        }

        public async Task DeleteCoffeeShopAsync(Guid id)
        {
            await DeleteAsync(id);
        }
    }
}
