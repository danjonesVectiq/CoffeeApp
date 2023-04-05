using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class BadgeRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public BadgeRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetBadgesContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("User", "/id");
        }

        public async Task<IEnumerable<Badge>> GetAllBadgesAsync()
        {
            var badgesContainer = await GetBadgesContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<Badge>(badgesContainer, "Badge");
        }

        public async Task<Badge> GetBadgeAsync(Guid id)
        {
            var badgesContainer = await GetBadgesContainerAsync();
            return await _cosmosDbService.GetItemAsync<Badge>(badgesContainer, id.ToString());
        }

        public async Task CreateBadgeAsync(Badge badge)
        {
            var badgesContainer = await GetBadgesContainerAsync();
            await _cosmosDbService.AddItemAsync(badgesContainer, badge);
        }

        public async Task UpdateBadgeAsync(Badge badge)
        {
            var badgesContainer = await GetBadgesContainerAsync();
            await _cosmosDbService.UpdateItemAsync(badgesContainer, badge.id.ToString(), badge);
        }

        public async Task DeleteBadgeAsync(Guid id)
        {
            var badgesContainer = await GetBadgesContainerAsync();
            await _cosmosDbService.DeleteItemAsync<Badge>(badgesContainer, id.ToString());
        }
    }
}
