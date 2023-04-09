using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using CoffeeAppAPI.Services;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(ICosmosDbRepository cosmosDbRepository)
        {
            _notificationService = new NotificationService(cosmosDbRepository);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(Guid id)
        {
            var notification = await _notificationService.GetNotificationAsync(id);

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
            await _notificationService.CreateNotificationAsync(notification);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.id }, notification);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateNotification(Guid id, [FromBody] Notification notification)
        {
            if (!ModelState.IsValid || id != notification.id)
            {
                return BadRequest(ModelState);
            }

            var existingNotification = await _notificationService.GetNotificationAsync(id);

            if (existingNotification == null)
            {
                return NotFound();
            }

            await _notificationService.UpdateNotificationAsync(notification);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotification(Guid id)
        {
            var existingNotification = await _notificationService.GetNotificationAsync(id);

            if (existingNotification == null)
            {
                return NotFound();
            }

            await _notificationService.DeleteNotificationAsync(id);
            return NoContent();
        }
    }
}
