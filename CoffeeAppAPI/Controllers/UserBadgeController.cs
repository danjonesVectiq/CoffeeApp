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
    public class UserBadgesController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public UserBadgesController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserBadge>>> GetAllUserBadges()
        {
            var userBadgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserBadges", "/id");
            var userBadges = await _cosmosDbService.GetAllItemsAsync<UserBadge>(userBadgesContainer);
            return Ok(userBadges);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserBadge>> GetUserBadge(Guid id)
        {
            var userBadgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserBadges", "/id");
            var userBadge = await _cosmosDbService.GetItemAsync<UserBadge>(userBadgesContainer, id.ToString());

            if (userBadge == null)
            {
                return NotFound();
            }

            return Ok(userBadge);
        }

        [HttpPost]
        public async Task<ActionResult<UserBadge>> CreateUserBadge([FromBody] UserBadge userBadge)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userBadge.Id = Guid.NewGuid();
            var userBadgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserBadges", "/id");
            await _cosmosDbService.AddItemAsync(userBadgesContainer, userBadge);
            return CreatedAtAction(nameof(GetUserBadge), new { id = userBadge.Id }, userBadge);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserBadge(Guid id, [FromBody] UserBadge userBadge)
        {
            if (!ModelState.IsValid || id != userBadge.Id)
            {
                return BadRequest(ModelState);
            }

            var userBadgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserBadges", "/id");
            var existingUserBadge = await _cosmosDbService.GetItemAsync<UserBadge>(userBadgesContainer, id.ToString());

            if (existingUserBadge == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(userBadgesContainer, id.ToString(), userBadge);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserBadge(Guid id)
        {
            var userBadgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserBadges", "/id");
            var existingUserBadge = await _cosmosDbService.GetItemAsync<UserBadge>(userBadgesContainer, id.ToString());

            if (existingUserBadge == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<UserBadge>(userBadgesContainer, id.ToString());
            return NoContent();
        }
    }
}
