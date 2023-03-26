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
    public class UserEventsController : ControllerBase
    {
        private readonly UserEventRepository _userEventRepository;

        public UserEventsController(ICosmosDbService cosmosDbService)
        {
            _userEventRepository = new UserEventRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEvent>>> GetAllUserEvents()
        {
            var userEvents = await _userEventRepository.GetAllUserEventsAsync();
            return Ok(userEvents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserEvent>> GetUserEvent(Guid id)
        {
            var userEvent = await _userEventRepository.GetUserEventAsync(id);

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
            await _userEventRepository.CreateUserEventAsync(userEvent);
            return CreatedAtAction(nameof(GetUserEvent), new { id = userEvent.id }, userEvent);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserEvent(Guid id, [FromBody] UserEvent userEvent)
        {
            if (!ModelState.IsValid || id != userEvent.id)
            {
                return BadRequest(ModelState);
            }

            var existingUserEvent = await _userEventRepository.GetUserEventAsync(id);

            if (existingUserEvent == null)
            {
                return NotFound();
            }

            await _userEventRepository.UpdateUserEventAsync(userEvent);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserEvent(Guid id)
        {
            var existingUserEvent = await _userEventRepository.GetUserEventAsync(id);

            if (existingUserEvent == null)
            {
                return NotFound();
            }

            await _userEventRepository.DeleteUserEventAsync(id);
            return NoContent();
        }
    }
}
