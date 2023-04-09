/* using System;
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
    public class UserFollowingsController : ControllerBase
    {
        private readonly UserFollowingService _userFollowingService;

        public UserFollowingsController(ICosmosDbRepository cosmosDbRepository)
        {
            _userFollowingService = new UserFollowingService(cosmosDbRepository);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserFollowing>>> GetAllUserFollowings()
        {
            var userFollowings = await _userFollowingService.GetAllUserFollowingsAsync();
            return Ok(userFollowings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserFollowing>> GetUserFollowing(Guid id)
        {
            var userFollowing = await _userFollowingService.GetUserFollowingAsync(id);

            if (userFollowing == null)
            {
                return NotFound();
            }

            return Ok(userFollowing);
        }

        [HttpPost]
        public async Task<ActionResult<UserFollowing>> CreateUserFollowing([FromBody] UserFollowing userFollowing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userFollowing.id = Guid.NewGuid();
            await _userFollowingService.CreateUserFollowingAsync(userFollowing);
            return CreatedAtAction(nameof(GetUserFollowing), new { id = userFollowing.id }, userFollowing);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserFollowing(Guid id, [FromBody] UserFollowing userFollowing)
        {
            if (!ModelState.IsValid || id != userFollowing.id)
            {
                return BadRequest(ModelState);
            }

            var existingUserFollowing = await _userFollowingService.GetUserFollowingAsync(id);

            if (existingUserFollowing == null)
            {
                return NotFound();
            }

            await _userFollowingService.UpdateUserFollowingAsync(userFollowing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserFollowing(Guid id)
        {
            var existingUserFollowing = await _userFollowingService.GetUserFollowingAsync(id);

            if (existingUserFollowing == null)
            {
                return NotFound();
            }

            await _userFollowingService.DeleteUserFollowingAsync(id);
            return NoContent();
        }
    }
}
 */