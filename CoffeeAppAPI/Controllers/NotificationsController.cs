using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public NotificationsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotifications()
        {
            var notificationsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Notifications", "/id");
            var notifications = await _cosmosDbService.GetAllItemsAsync<Notification>(notificationsContainer);
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(Guid id)
        {
            var notificationsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Notifications", "/id");
            var notification = await _cosmosDbService.GetItemAsync<Notification>(notificationsContainer, id.ToString());

            if (notification == null)
            {
                return NotFound();
            }

            return Ok(notification);
        }

        [HttpPost]
        public async Task<ActionResult<Notification>> CreateNotification([FromBody] Notification notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            notification.id = Guid.NewGuid();
            var notificationsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Notifications", "/id");
            await _cosmosDbService.AddItemAsync(notificationsContainer, notification);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.id }, notification);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateNotification(Guid id, [FromBody] Notification notification)
        {
            if (!ModelState.IsValid || id != notification.id)
            {
                return BadRequest(ModelState);
            }

            var notificationsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Notifications", "/id");
            var existingNotification = await _cosmosDbService.GetItemAsync<Notification>(notificationsContainer, id.ToString());

            if (existingNotification == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(notificationsContainer, id.ToString(), notification);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotification(Guid id)
        {
            var notificationsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Notifications", "/id");
            var existingNotification = await _cosmosDbService.GetItemAsync<Notification>(notificationsContainer, id.ToString());

            if (existingNotification == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<Notification>(notificationsContainer, id.ToString());
            return NoContent();
        }
    }
}
