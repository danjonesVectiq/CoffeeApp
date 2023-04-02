/* using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class UserFollowingRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public UserFollowingRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetUserFollowingsContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("UserFollowings", "/id");
        }

        public async Task<IEnumerable<UserFollowing>> GetAllUserFollowingsAsync()
        {
            var userFollowingsContainer = await GetUserFollowingsContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<UserFollowing>(userFollowingsContainer);
        }

        public async Task<UserFollowing> GetUserFollowingAsync(Guid id)
        {
            var userFollowingsContainer = await GetUserFollowingsContainerAsync();
            return await _cosmosDbService.GetItemAsync<UserFollowing>(userFollowingsContainer, id.ToString());
        }

        public async Task CreateUserFollowingAsync(UserFollowing userFollowing)
        {
            var userFollowingsContainer = await GetUserFollowingsContainerAsync();
            await _cosmosDbService.AddItemAsync(userFollowingsContainer, userFollowing);
        }

        public async Task UpdateUserFollowingAsync(UserFollowing userFollowing)
        {
            var userFollowingsContainer = await GetUserFollowingsContainerAsync();
            await _cosmosDbService.UpdateItemAsync(userFollowingsContainer, userFollowing.id.ToString(), userFollowing);
        }

        public async Task DeleteUserFollowingAsync(Guid id)
        {
            var userFollowingsContainer = await GetUserFollowingsContainerAsync();
            await _cosmosDbService.DeleteItemAsync<UserFollowing>(userFollowingsContainer, id.ToString());
        }
    }
}
 */