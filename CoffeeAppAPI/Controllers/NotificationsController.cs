using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using CoffeeAppAPI.Repositories;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationRepository _notificationRepository;

        public NotificationsController(ICosmosDbService cosmosDbService)
        {
            _notificationRepository = new NotificationRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotifications()
        {
            var notifications = await _notificationRepository.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(Guid id)
        {
            var notification = await _notificationRepository.GetNotificationAsync(id);

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
            await _notificationRepository.CreateNotificationAsync(notification);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.id }, notification);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateNotification(Guid id, [FromBody] Notification notification)
        {
            if (!ModelState.IsValid || id != notification.id)
            {
                return BadRequest(ModelState);
            }

            var existingNotification = await _notificationRepository.GetNotificationAsync(id);

            if (existingNotification == null)
            {
                return NotFound();
            }

            await _notificationRepository.UpdateNotificationAsync(notification);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotification(Guid id)
        {
            var existingNotification = await _notificationRepository.GetNotificationAsync(id);

            if (existingNotification == null)
            {
                return NotFound();
            }

            await _notificationRepository.DeleteNotificationAsync(id);
            return NoContent();
        }
    }
}
