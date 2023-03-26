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
    public class BadgesController : ControllerBase
    {
        private readonly BadgeRepository _badgeRepository;

        public BadgesController(ICosmosDbService cosmosDbService)
        {
            _badgeRepository = new BadgeRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Badge>>> GetAllBadges()
        {
            var badges = await _badgeRepository.GetAllBadgesAsync();
            return Ok(badges);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Badge>> GetBadge(Guid id)
        {
            var badge = await _badgeRepository.GetBadgeAsync(id);

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

            badge.id = Guid.NewGuid();
            await _badgeRepository.CreateBadgeAsync(badge);
            return CreatedAtAction(nameof(GetBadge), new { id = badge.id }, badge);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBadge(Guid id, [FromBody] Badge badge)
        {
            if (!ModelState.IsValid || id != badge.id)
            {
                return BadRequest(ModelState);
            }

            var existingBadge = await _badgeRepository.GetBadgeAsync(id);

            if (existingBadge == null)
            {
                return NotFound();
            }

            await _badgeRepository.UpdateBadgeAsync(badge);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBadge(Guid id)
        {
            var existingBadge = await _badgeRepository.GetBadgeAsync(id);

            if (existingBadge == null)
            {
                return NotFound();
            }

            await _badgeRepository.DeleteBadgeAsync(id);
            return NoContent();
        }
    }
}
