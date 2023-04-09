/* using System;
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
    public class EventsController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventsController(ICosmosDbRepository cosmosDbRepository)
        {
            _eventService = new EventService(cosmosDbRepository);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(Guid id)
        {
            var eventItem = await _eventService.GetEventAsync(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return Ok(eventItem);
        }

        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event eventItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            eventItem.id = Guid.NewGuid();
            await _eventService.CreateEventAsync(eventItem);
            return CreatedAtAction(nameof(GetEvent), new { id = eventItem.id }, eventItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] Event eventItem)
        {
            if (!ModelState.IsValid || id != eventItem.id)
            {
                return BadRequest(ModelState);
            }

            var existingEvent = await _eventService.GetEventAsync(id);

            if (existingEvent == null)
            {
                return NotFound();
            }

            await _eventService.UpdateEventAsync(eventItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(Guid id)
        {
            var existingEvent = await _eventService.GetEventAsync(id);

            if (existingEvent == null)
            {
                return NotFound();
            }

            await _eventService.DeleteEventAsync(id);
            return NoContent();
        }
    }
}
 */