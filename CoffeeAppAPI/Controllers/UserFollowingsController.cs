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
    public class UserFollowingsController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public UserFollowingsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserFollowing>>> GetAllUserFollowings()
        {
            var userFollowingsContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserFollowings", "/id");
            var userFollowings = await _cosmosDbService.GetAllItemsAsync<UserFollowing>(userFollowingsContainer);
            return Ok(userFollowings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserFollowing>> GetUserFollowing(Guid id)
        {
            var userFollowingsContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserFollowings", "/id");
            var userFollowing = await _cosmosDbService.GetItemAsync<UserFollowing>(userFollowingsContainer, id.ToString());

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
            var userFollowingsContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserFollowings", "/id");
            await _cosmosDbService.AddItemAsync(userFollowingsContainer, userFollowing);
            return CreatedAtAction(nameof(GetUserFollowing), new { id = userFollowing.id }, userFollowing);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserFollowing(Guid id, [FromBody] UserFollowing userFollowing)
        {
            if (!ModelState.IsValid || id != userFollowing.id)
            {
                return BadRequest(ModelState);
            }

            var userFollowingsContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserFollowings", "/id");
            var existingUserFollowing = await _cosmosDbService.GetItemAsync<UserFollowing>(userFollowingsContainer, id.ToString());

            if (existingUserFollowing == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(userFollowingsContainer, id.ToString(), userFollowing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserFollowing(Guid id)
        {
            var userFollowingsContainer = await _cosmosDbService.GetOrCreateContainerAsync("UserFollowings", "/id");
            var existingUserFollowing = await _cosmosDbService.GetItemAsync<UserFollowing>(userFollowingsContainer, id.ToString());

            if (existingUserFollowing == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<UserFollowing>(userFollowingsContainer, id.ToString());
            return NoContent();
        }
    }
}
