using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface IRoasterRepository : IRepository<Roaster>
    {
    }

    public class RoasterRepository : CosmosDbRepository<Roaster>, IRoasterRepository
    {
        public RoasterRepository(ICosmosDbService cosmosDbService)
            : base(cosmosDbService, "Coffee", "/id", "Roaster")
        {
        }

        public async Task<IEnumerable<Roaster>> GetAllRoastersAsync()
        {
            return await GetAllAsync();
        }

        public async Task<Roaster> GetRoasterAsync(Guid id)
        {
            return await GetAsync(id);
        }

        public async Task CreateRoasterAsync(Roaster roaster)
        {
            await CreateAsync(roaster);
        }

        public async Task UpdateRoasterAsync(Roaster roaster)
        {
            await UpdateAsync(roaster);
        }

        public async Task DeleteRoasterAsync(Guid id)
        {
            await DeleteAsync(id);
        }
    }
}
