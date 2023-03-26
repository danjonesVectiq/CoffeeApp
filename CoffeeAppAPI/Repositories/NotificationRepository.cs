using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class NotificationRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public NotificationRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetNotificationsContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("Notifications", "/id");
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            var notificationsContainer = await GetNotificationsContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<Notification>(notificationsContainer);
        }

        public async Task<Notification> GetNotificationAsync(Guid id)
        {
            var notificationsContainer = await GetNotificationsContainerAsync();
            return await _cosmosDbService.GetItemAsync<Notification>(notificationsContainer, id.ToString());
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            var notificationsContainer = await GetNotificationsContainerAsync();
            await _cosmosDbService.AddItemAsync(notificationsContainer, notification);
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            var notificationsContainer = await GetNotificationsContainerAsync();
            await _cosmosDbService.UpdateItemAsync(notificationsContainer, notification.id.ToString(), notification);
        }

        public async Task DeleteNotificationAsync(Guid id)
        {
            var notificationsContainer = await GetNotificationsContainerAsync();
            await _cosmosDbService.DeleteItemAsync<Notification>(notificationsContainer, id.ToString());
        }
    }
}
