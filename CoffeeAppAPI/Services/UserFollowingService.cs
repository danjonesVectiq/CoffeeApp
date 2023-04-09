/* using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public class UserFollowingService
    {
        private readonly ICosmosDbRepository _cosmosDbRepository;

        public UserFollowingService(ICosmosDbRepository cosmosDbRepository)
        {
            _cosmosDbRepository = cosmosDbRepository;
        }

        public async Task<Container> GetUserFollowingsContainerAsync()
        {
            return await _cosmosDbRepository.GetOrCreateContainerAsync("UserFollowings", "/id");
        }

        public async Task<IEnumerable<UserFollowing>> GetAllUserFollowingsAsync()
        {
            var userFollowingsContainer = await GetUserFollowingsContainerAsync();
            return await _cosmosDbRepository.GetAllItemsAsync<UserFollowing>(userFollowingsContainer);
        }

        public async Task<UserFollowing> GetUserFollowingAsync(Guid id)
        {
            var userFollowingsContainer = await GetUserFollowingsContainerAsync();
            return await _cosmosDbRepository.GetItemAsync<UserFollowing>(userFollowingsContainer, id.ToString());
        }

        public async Task CreateUserFollowingAsync(UserFollowing userFollowing)
        {
            var userFollowingsContainer = await GetUserFollowingsContainerAsync();
            await _cosmosDbRepository.AddItemAsync(userFollowingsContainer, userFollowing);
        }

        public async Task UpdateUserFollowingAsync(UserFollowing userFollowing)
        {
            var userFollowingsContainer = await GetUserFollowingsContainerAsync();
            await _cosmosDbRepository.UpdateItemAsync(userFollowingsContainer, userFollowing.id.ToString(), userFollowing);
        }

        public async Task DeleteUserFollowingAsync(Guid id)
        {
            var userFollowingsContainer = await GetUserFollowingsContainerAsync();
            await _cosmosDbRepository.DeleteItemAsync<UserFollowing>(userFollowingsContainer, id.ToString());
        }
    }
}
 */