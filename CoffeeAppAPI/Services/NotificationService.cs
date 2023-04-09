using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public class NotificationService
    {
        private readonly ICosmosDbRepository _cosmosDbRepository;

        public NotificationService(ICosmosDbRepository cosmosDbRepository)
        {
            _cosmosDbRepository = cosmosDbRepository;
        }

        public async Task<Container> GetNotificationsContainerAsync()
        {
            return await _cosmosDbRepository.GetOrCreateContainerAsync("User", "/id");
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            var notificationsContainer = await GetNotificationsContainerAsync();
            return await _cosmosDbRepository.GetAllItemsAsync<Notification>(notificationsContainer, "Notification");
        }

        public async Task<Notification> GetNotificationAsync(Guid id)
        {
            var notificationsContainer = await GetNotificationsContainerAsync();
            return await _cosmosDbRepository.GetItemAsync<Notification>(notificationsContainer, id.ToString());
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            var notificationsContainer = await GetNotificationsContainerAsync();
            await _cosmosDbRepository.AddItemAsync(notificationsContainer, notification);
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            var notificationsContainer = await GetNotificationsContainerAsync();
            await _cosmosDbRepository.UpdateItemAsync(notificationsContainer, notification.id.ToString(), notification);
        }

        public async Task DeleteNotificationAsync(Guid id)
        {
            var notificationsContainer = await GetNotificationsContainerAsync();
            await _cosmosDbRepository.DeleteItemAsync<Notification>(notificationsContainer, id.ToString());
        }
    }
}
