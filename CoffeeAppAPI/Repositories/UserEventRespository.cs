using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class UserEventRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public UserEventRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetUserEventsContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("UserEvents", "/id");
        }

        public async Task<IEnumerable<UserEvent>> GetAllUserEventsAsync()
        {
            var userEventsContainer = await GetUserEventsContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<UserEvent>(userEventsContainer);
        }

        public async Task<UserEvent> GetUserEventAsync(Guid id)
        {
            var userEventsContainer = await GetUserEventsContainerAsync();
            return await _cosmosDbService.GetItemAsync<UserEvent>(userEventsContainer, id.ToString());
        }

        public async Task CreateUserEventAsync(UserEvent userEvent)
        {
            var userEventsContainer = await GetUserEventsContainerAsync();
            await _cosmosDbService.AddItemAsync(userEventsContainer, userEvent);
        }

        public async Task UpdateUserEventAsync(UserEvent userEvent)
        {
            var userEventsContainer = await GetUserEventsContainerAsync();
            await _cosmosDbService.UpdateItemAsync(userEventsContainer, userEvent.id.ToString(), userEvent);
        }

        public async Task DeleteUserEventAsync(Guid id)
        {
            var userEventsContainer = await GetUserEventsContainerAsync();
            await _cosmosDbService.DeleteItemAsync<UserEvent>(userEventsContainer, id.ToString());
        }
    }
}
