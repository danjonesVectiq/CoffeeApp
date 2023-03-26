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
    public class EventsController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public EventsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
        {
            var eventsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Events", "/id");
            var events = await _cosmosDbService.GetAllItemsAsync<Event>(eventsContainer);
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(Guid id)
        {
            var eventsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Events", "/id");
            var eventItem = await _cosmosDbService.GetItemAsync<Event>(eventsContainer, id.ToString());

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
            var eventsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Events", "/id");
            await _cosmosDbService.AddItemAsync(eventsContainer, eventItem);
            return CreatedAtAction(nameof(GetEvent), new { id = eventItem.id }, eventItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] Event eventItem)
        {
            if (!ModelState.IsValid || id != eventItem.id)
            {
                return BadRequest(ModelState);
            }

            var eventsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Events", "/id");
            var existingEvent = await _cosmosDbService.GetItemAsync<Event>(eventsContainer, id.ToString());

            if (existingEvent == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(eventsContainer, id.ToString(), eventItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEvent(Guid id)
        {
            var eventsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Events", "/id");
            var existingEvent = await _cosmosDbService.GetItemAsync<Event>(eventsContainer, id.ToString());

            if (existingEvent == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<Event>(eventsContainer, id.ToString());
            return NoContent();
        }
    }
}
