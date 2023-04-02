/* using System;
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
    public class EventsController : ControllerBase
    {
        private readonly EventRepository _eventRepository;

        public EventsController(ICosmosDbService cosmosDbService)
        {
            _eventRepository = new EventRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(Guid id)
        {
            var eventItem = await _eventRepository.GetEventAsync(id);

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
            await _eventRepository.CreateEventAsync(eventItem);
            return CreatedAtAction(nameof(GetEvent), new { id = eventItem.id }, eventItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] Event eventItem)
        {
            if (!ModelState.IsValid || id != eventItem.id)
            {
                return BadRequest(ModelState);
            }

            var existingEvent = await _eventRepository.GetEventAsync(id);

            if (existingEvent == null)
            {
                return NotFound();
            }

            await _eventRepository.UpdateEventAsync(eventItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(Guid id)
        {
            var existingEvent = await _eventRepository.GetEventAsync(id);

            if (existingEvent == null)
            {
                return NotFound();
            }

            await _eventRepository.DeleteEventAsync(id);
            return NoContent();
        }
    }
}
 */