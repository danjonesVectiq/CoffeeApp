using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class UserBadgeRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public UserBadgeRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetUserBadgesContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("UserBadges", "/id");
        }

        public async Task<IEnumerable<UserBadge>> GetAllUserBadgesAsync()
        {
            var userBadgesContainer = await GetUserBadgesContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<UserBadge>(userBadgesContainer);
        }

        public async Task<UserBadge> GetUserBadgeAsync(Guid id)
        {
            var userBadgesContainer = await GetUserBadgesContainerAsync();
            return await _cosmosDbService.GetItemAsync<UserBadge>(userBadgesContainer, id.ToString());
        }

        public async Task CreateUserBadgeAsync(UserBadge userBadge)
        {
            var userBadgesContainer = await GetUserBadgesContainerAsync();
            await _cosmosDbService.AddItemAsync(userBadgesContainer, userBadge);
        }

        public async Task UpdateUserBadgeAsync(UserBadge userBadge)
        {
            var userBadgesContainer = await GetUserBadgesContainerAsync();
            await _cosmosDbService.UpdateItemAsync(userBadgesContainer, userBadge.id.ToString(), userBadge);
        }

        public async Task DeleteUserBadgeAsync(Guid id)
        {
            var userBadgesContainer = await GetUserBadgesContainerAsync();
            await _cosmosDbService.DeleteItemAsync<UserBadge>(userBadgesContainer, id.ToString());
        }
    }
}
