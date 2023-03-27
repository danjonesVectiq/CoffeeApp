using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class RoasterRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public RoasterRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetRoastersContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("Roasters", "/id");
        }

        public async Task<IEnumerable<Roaster>> GetAllRoastersAsync()
        {
            var roastersContainer = await GetRoastersContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<Roaster>(roastersContainer);
        }

        public async Task<Roaster> GetRoasterAsync(Guid id)
        {
            var roastersContainer = await GetRoastersContainerAsync();
            return await _cosmosDbService.GetItemAsync<Roaster>(roastersContainer, id.ToString());
        }

        public async Task CreateRoasterAsync(Roaster roaster)
        {
            var roastersContainer = await GetRoastersContainerAsync();
            await _cosmosDbService.AddItemAsync(roastersContainer, roaster);
        }

        public async Task UpdateRoasterAsync(Roaster roaster)
        {
            var roastersContainer = await GetRoastersContainerAsync();
            await _cosmosDbService.UpdateItemAsync(roastersContainer, roaster.id.ToString(), roaster);
        }

        public async Task DeleteRoasterAsync(Guid id)
        {
            var roastersContainer = await GetRoastersContainerAsync();
            await _cosmosDbService.DeleteItemAsync<Roaster>(roastersContainer, id.ToString());
        }
    }
}
