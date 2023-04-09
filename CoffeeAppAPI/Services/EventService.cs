/* using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public class EventService
    {
        private readonly ICosmosDbRepository _cosmosDbRepository;

        public EventService(ICosmosDbRepository cosmosDbRepository)
        {
            _cosmosDbRepository = cosmosDbRepository;
        }

        public async Task<Container> GetEventsContainerAsync()
        {
            return await _cosmosDbRepository.GetOrCreateContainerAsync("Events", "/id");
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            var eventsContainer = await GetEventsContainerAsync();
            return await _cosmosDbRepository.GetAllItemsAsync<Event>(eventsContainer);
        }

        public async Task<Event> GetEventAsync(Guid id)
        {
            var eventsContainer = await GetEventsContainerAsync();
            return await _cosmosDbRepository.GetItemAsync<Event>(eventsContainer, id.ToString());
        }

        public async Task CreateEventAsync(Event eventItem)
        {
            var eventsContainer = await GetEventsContainerAsync();
            await _cosmosDbRepository.AddItemAsync(eventsContainer, eventItem);
        }

        public async Task UpdateEventAsync(Event eventItem)
        {
            var eventsContainer = await GetEventsContainerAsync();
            await _cosmosDbRepository.UpdateItemAsync(eventsContainer, eventItem.id.ToString(), eventItem);
        }

        public async Task DeleteEventAsync(Guid id)
        {
            var eventsContainer = await GetEventsContainerAsync();
            await _cosmosDbRepository.DeleteItemAsync<Event>(eventsContainer, id.ToString());
        }
    }
}
 */