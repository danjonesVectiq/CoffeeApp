using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface ICoffeeRepository : IRepository<Coffee>
    {
        // Add any Coffee-specific methods here, if needed
    }

    public class CoffeeRepository : CosmosDbRepository<Coffee>, ICoffeeRepository
    {
        public CoffeeRepository(ICosmosDbService cosmosDbService)
            : base(cosmosDbService, "Coffee", "/id", "Coffee")
        {
        }

        // Implement any Coffee-specific methods here, if needed

        public async Task<IEnumerable<Coffee>> GetAllCoffeesAsync()
        {
            return await GetAllAsync();
        }

        public async Task<Coffee> GetCoffeeAsync(Guid id)
        {
            return await GetAsync(id);
        }

        public async Task CreateCoffeeAsync(Coffee coffee)
        {
            await CreateAsync(coffee);
        }

        public async Task UpdateCoffeeAsync(Coffee coffee)
        {
            await UpdateAsync(coffee);
        }

        public async Task DeleteCoffeeAsync(Guid id)
        {
            await DeleteAsync(id);
        }
    }
}