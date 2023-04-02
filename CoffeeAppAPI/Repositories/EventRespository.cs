/* using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class EventRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public EventRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetEventsContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("Events", "/id");
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            var eventsContainer = await GetEventsContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<Event>(eventsContainer);
        }

        public async Task<Event> GetEventAsync(Guid id)
        {
            var eventsContainer = await GetEventsContainerAsync();
            return await _cosmosDbService.GetItemAsync<Event>(eventsContainer, id.ToString());
        }

        public async Task CreateEventAsync(Event eventItem)
        {
            var eventsContainer = await GetEventsContainerAsync();
            await _cosmosDbService.AddItemAsync(eventsContainer, eventItem);
        }

        public async Task UpdateEventAsync(Event eventItem)
        {
            var eventsContainer = await GetEventsContainerAsync();
            await _cosmosDbService.UpdateItemAsync(eventsContainer, eventItem.id.ToString(), eventItem);
        }

        public async Task DeleteEventAsync(Guid id)
        {
            var eventsContainer = await GetEventsContainerAsync();
            await _cosmosDbService.DeleteItemAsync<Event>(eventsContainer, id.ToString());
        }
    }
}
 */