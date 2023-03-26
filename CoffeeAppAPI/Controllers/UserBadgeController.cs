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
    public class UserBadgesController : ControllerBase
    {
        private readonly UserBadgeRepository _userUserBadgeRepository;

        public UserBadgesController(ICosmosDbService cosmosDbService)
        {
            _userUserBadgeRepository = new UserBadgeRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserBadge>>> GetAllUserBadges()
        {
            var userUserBadges = await _userUserBadgeRepository.GetAllUserBadgesAsync();
            return Ok(userUserBadges);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserBadge>> GetUserBadge(Guid id)
        {
            var userUserBadge = await _userUserBadgeRepository.GetUserBadgeAsync(id);

            if (userUserBadge == null)
            {
                return NotFound();
            }

            return Ok(userUserBadge);
        }

        [HttpPost]
        public async Task<ActionResult<UserBadge>> CreateUserBadge([FromBody] UserBadge userUserBadge)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userUserBadge.id = Guid.NewGuid();
            await _userUserBadgeRepository.CreateUserBadgeAsync(userUserBadge);
            return CreatedAtAction(nameof(GetUserBadge), new { id = userUserBadge.id }, userUserBadge);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserBadge(Guid id, [FromBody] UserBadge userUserBadge)
        {
            if (!ModelState.IsValid || id != userUserBadge.id)
            {
                return BadRequest(ModelState);
            }

            var existingUserBadge = await _userUserBadgeRepository.GetUserBadgeAsync(id);

            if (existingUserBadge == null)
            {
                return NotFound();
            }

            await _userUserBadgeRepository.UpdateUserBadgeAsync(userUserBadge);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserBadge(Guid id)
        {
            var existingUserBadge = await _userUserBadgeRepository.GetUserBadgeAsync(id);

            if (existingUserBadge == null)
            {
                return NotFound();
            }

            await _userUserBadgeRepository.DeleteUserBadgeAsync(id);
            return NoContent();
        }
    }
}
