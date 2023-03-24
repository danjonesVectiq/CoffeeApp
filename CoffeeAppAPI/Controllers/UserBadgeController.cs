using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeApp.Models;
using CoffeeApp.Services;

namespace CoffeeApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserBadgesController : ControllerBase
    {
        private readonly CosmosDbService _cosmosDbService;

        public UserBadgesController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserBadge>>> GetAllUserBadges()
        {
            var userBadgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserBadges", "/userBadgeId");
            var userBadges = await _cosmosDbService.GetAllItemsAsync<UserBadge>(userBadgesContainer);
            return Ok(userBadges);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserBadge>> GetUserBadge(Guid id)
        {
            var userBadgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserBadges", "/userBadgeId");
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

            userBadge.UserBadgeId = Guid.NewGuid();
            var userBadgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserBadges", "/userBadgeId");
            await _cosmosDbService.AddItemAsync(userBadgesContainer, userBadge);
            return CreatedAtAction(nameof(GetUserBadge), new { id = userBadge.UserBadgeId }, userBadge);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserBadge(Guid id, [FromBody] UserBadge userBadge)
        {
            if (!ModelState.IsValid || id != userBadge.UserBadgeId)
            {
                return BadRequest(ModelState);
            }

            var userBadgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserBadges", "/userBadgeId");
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
            var userBadgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserBadges", "/userBadgeId");
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
