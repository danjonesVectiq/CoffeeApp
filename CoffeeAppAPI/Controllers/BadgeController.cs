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
    public class BadgesController : ControllerBase
    {
        private readonly CosmosDbService _cosmosDbService;

        public BadgesController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Badge>>> GetAllBadges()
        {
            var badgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("Badges", "/id");
            var badges = await _cosmosDbService.GetAllItemsAsync<Badge>(badgesContainer);
            return Ok(badges);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Badge>> GetBadge(Guid id)
        {
            var badgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("Badges", "/id");
            var badge = await _cosmosDbService.GetItemAsync<Badge>(badgesContainer, id.ToString());

            if (badge == null)
            {
                return NotFound();
            }

            return Ok(badge);
        }

        [HttpPost]
        public async Task<ActionResult<Badge>> CreateBadge([FromBody] Badge badge)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            badge.Id = Guid.NewGuid();
            var badgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("Badges", "/id");
            await _cosmosDbService.AddItemAsync(badgesContainer, badge);
            return CreatedAtAction(nameof(GetBadge), new { id = badge.Id }, badge);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBadge(Guid id, [FromBody] Badge badge)
        {
            if (!ModelState.IsValid || id != badge.Id)
            {
                return BadRequest(ModelState);
            }

            var badgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("Badges", "/id");
            var existingBadge = await _cosmosDbService.GetItemAsync<Badge>(badgesContainer, id.ToString());

            if (existingBadge == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(badgesContainer, id.ToString(), badge);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBadge(Guid id)
        {
            var badgesContainer = await _cosmosDbService.GetOrCreateContainerAsync("Badges", "/id");
            var existingBadge = await _cosmosDbService.GetItemAsync<Badge>(badgesContainer, id.ToString());

            if (existingBadge == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<Badge>(badgesContainer, id.ToString());
            return NoContent();
        }
    }
}
