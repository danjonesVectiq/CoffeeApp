/* using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class FriendRequestRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public FriendRequestRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetFriendRequestsContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("FriendRequests", "/id");
        }

        public async Task<IEnumerable<FriendRequest>> GetAllFriendRequestsAsync()
        {
            var friendRequestsContainer = await GetFriendRequestsContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<FriendRequest>(friendRequestsContainer);
        }

        public async Task<FriendRequest> GetFriendRequestAsync(Guid id)
        {
            var friendRequestsContainer = await GetFriendRequestsContainerAsync();
            return await _cosmosDbService.GetItemAsync<FriendRequest>(friendRequestsContainer, id.ToString());
        }

        public async Task CreateFriendRequestAsync(FriendRequest friendRequest)
        {
            var friendRequestsContainer = await GetFriendRequestsContainerAsync();
            await _cosmosDbService.AddItemAsync(friendRequestsContainer, friendRequest);
        }

        public async Task UpdateFriendRequestAsync(FriendRequest friendRequest)
        {
            var friendRequestsContainer = await GetFriendRequestsContainerAsync();
            await _cosmosDbService.UpdateItemAsync(friendRequestsContainer, friendRequest.id.ToString(), friendRequest);
        }

        public async Task DeleteFriendRequestAsync(Guid id)
        {
            var friendRequestsContainer = await GetFriendRequestsContainerAsync();
            await _cosmosDbService.DeleteItemAsync<FriendRequest>(friendRequestsContainer, id.ToString());
        }
    }
}
 */