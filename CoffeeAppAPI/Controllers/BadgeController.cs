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
        private readonly IBadgeRepository _badgeRepository;

        public BadgesController(IBadgeRepository badgeRepository)
        {
            _badgeRepository = badgeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Badge>>> GetAllBadges()
        {
            var badges = await _badgeRepository.GetAllAsync();
            return Ok(badges);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Badge>> GetBadge(Guid id)
        {
            var badge = await _badgeRepository.GetAsync(id);

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
            await _badgeRepository.CreateAsync(badge);
            return CreatedAtAction(nameof(GetBadge), new { id = badge.id }, badge);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBadge(Guid id, [FromBody] Badge badge)
        {
            if (!ModelState.IsValid || id != badge.id)
            {
                return BadRequest(ModelState);
            }

            var existingBadge = await _badgeRepository.GetAsync(id);

            if (existingBadge == null)
            {
                return NotFound();
            }

            await _badgeRepository.UpdateAsync(badge);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBadge(Guid id)
        {
            var existingBadge = await _badgeRepository.GetAsync(id);

            if (existingBadge == null)
            {
                return NotFound();
            }

            await _badgeRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
