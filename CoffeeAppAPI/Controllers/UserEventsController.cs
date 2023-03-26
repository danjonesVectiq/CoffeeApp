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
    public class UserEventsController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public UserEventsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEvent>>> GetAllUserEvents()
        {
            var userEventsContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserEvents", "/id");
            var userEvents = await _cosmosDbService.GetAllItemsAsync<UserEvent>(userEventsContainer);
            return Ok(userEvents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserEvent>> GetUserEvent(Guid id)
        {
            var userEventsContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserEvents", "/id");
            var userEvent = await _cosmosDbService.GetItemAsync<UserEvent>(userEventsContainer, id.ToString());

            if (userEvent == null)
            {
                return NotFound();
            }

            return Ok(userEvent);
        }

        [HttpPost]
        public async Task<ActionResult<UserEvent>> CreateUserEvent([FromBody] UserEvent userEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userEvent.id = Guid.NewGuid();
            var userEventsContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserEvents", "/id");
            await _cosmosDbService.AddItemAsync(userEventsContainer, userEvent);
            return CreatedAtAction(nameof(GetUserEvent), new { id = userEvent.id }, userEvent);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserEvent(Guid id, [FromBody] UserEvent userEvent)
        {
            if (!ModelState.IsValid || id != userEvent.id)
            {
                return BadRequest(ModelState);
            }

            var userEventsContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserEvents", "/id");
            var existingUserEvent = await _cosmosDbService.GetItemAsync<UserEvent>(userEventsContainer, id.ToString());

            if (existingUserEvent == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(userEventsContainer, id.ToString(), userEvent);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserEvent(Guid id)
        {
            var userEventsContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserEvents", "/id");
            var existingUserEvent = await _cosmosDbService.GetItemAsync<UserEvent>(userEventsContainer, id.ToString());

            if (existingUserEvent == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<UserEvent>(userEventsContainer, id.ToString());
            return NoContent();
        }
    }
}
