using System;
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
    public class BadgesController : ControllerBase
    {
        private readonly IBadgeService _badgeService;

        public BadgesController(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Badge>>> GetAllBadges()
        {
            var badges = await _badgeService.GetAllAsync();
            return Ok(badges);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Badge>> GetBadge(Guid id)
        {
            var badge = await _badgeService.GetAsync(id);

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
            await _badgeService.CreateAsync(badge);
            return CreatedAtAction(nameof(GetBadge), new { id = badge.id }, badge);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBadge(Guid id, [FromBody] Badge badge)
        {
            if (!ModelState.IsValid || id != badge.id)
            {
                return BadRequest(ModelState);
            }

            var existingBadge = await _badgeService.GetAsync(id);

            if (existingBadge == null)
            {
                return NotFound();
            }

            await _badgeService.UpdateAsync(badge);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBadge(Guid id)
        {
            var existingBadge = await _badgeService.GetAsync(id);

            if (existingBadge == null)
            {
                return NotFound();
            }

            await _badgeService.DeleteAsync(id);
            return NoContent();
        }
    }
}
