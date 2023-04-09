/* using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public class FriendRequestService
    {
        private readonly ICosmosDbRepository _cosmosDbRepository;

        public FriendRequestService(ICosmosDbRepository cosmosDbRepository)
        {
            _cosmosDbRepository = cosmosDbRepository;
        }

        public async Task<Container> GetFriendRequestsContainerAsync()
        {
            return await _cosmosDbRepository.GetOrCreateContainerAsync("FriendRequests", "/id");
        }

        public async Task<IEnumerable<FriendRequest>> GetAllFriendRequestsAsync()
        {
            var friendRequestsContainer = await GetFriendRequestsContainerAsync();
            return await _cosmosDbRepository.GetAllItemsAsync<FriendRequest>(friendRequestsContainer);
        }

        public async Task<FriendRequest> GetFriendRequestAsync(Guid id)
        {
            var friendRequestsContainer = await GetFriendRequestsContainerAsync();
            return await _cosmosDbRepository.GetItemAsync<FriendRequest>(friendRequestsContainer, id.ToString());
        }

        public async Task CreateFriendRequestAsync(FriendRequest friendRequest)
        {
            var friendRequestsContainer = await GetFriendRequestsContainerAsync();
            await _cosmosDbRepository.AddItemAsync(friendRequestsContainer, friendRequest);
        }

        public async Task UpdateFriendRequestAsync(FriendRequest friendRequest)
        {
            var friendRequestsContainer = await GetFriendRequestsContainerAsync();
            await _cosmosDbRepository.UpdateItemAsync(friendRequestsContainer, friendRequest.id.ToString(), friendRequest);
        }

        public async Task DeleteFriendRequestAsync(Guid id)
        {
            var friendRequestsContainer = await GetFriendRequestsContainerAsync();
            await _cosmosDbRepository.DeleteItemAsync<FriendRequest>(friendRequestsContainer, id.ToString());
        }
    }
}
 */