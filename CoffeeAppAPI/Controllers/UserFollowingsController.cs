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
    public class UserFollowingsController : ControllerBase
    {
        private readonly UserFollowingRepository _userFollowingRepository;

        public UserFollowingsController(ICosmosDbService cosmosDbService)
        {
            _userFollowingRepository = new UserFollowingRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserFollowing>>> GetAllUserFollowings()
        {
            var userFollowings = await _userFollowingRepository.GetAllUserFollowingsAsync();
            return Ok(userFollowings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserFollowing>> GetUserFollowing(Guid id)
        {
            var userFollowing = await _userFollowingRepository.GetUserFollowingAsync(id);

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
            await _userFollowingRepository.CreateUserFollowingAsync(userFollowing);
            return CreatedAtAction(nameof(GetUserFollowing), new { id = userFollowing.id }, userFollowing);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserFollowing(Guid id, [FromBody] UserFollowing userFollowing)
        {
            if (!ModelState.IsValid || id != userFollowing.id)
            {
                return BadRequest(ModelState);
            }

            var existingUserFollowing = await _userFollowingRepository.GetUserFollowingAsync(id);

            if (existingUserFollowing == null)
            {
                return NotFound();
            }

            await _userFollowingRepository.UpdateUserFollowingAsync(userFollowing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserFollowing(Guid id)
        {
            var existingUserFollowing = await _userFollowingRepository.GetUserFollowingAsync(id);

            if (existingUserFollowing == null)
            {
                return NotFound();
            }

            await _userFollowingRepository.DeleteUserFollowingAsync(id);
            return NoContent();
        }
    }
}
