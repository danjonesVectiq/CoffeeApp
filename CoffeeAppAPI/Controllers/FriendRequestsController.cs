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
    public class FriendRequestsController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public FriendRequestsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FriendRequest>>> GetAllFriendRequests()
        {
            var friendRequestsContainer = await _cosmosDbService.GetOrCreateContainerAsync("FriendRequests", "/id");
            var friendRequests = await _cosmosDbService.GetAllItemsAsync<FriendRequest>(friendRequestsContainer);
            return Ok(friendRequests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FriendRequest>> GetFriendRequest(Guid id)
        {
            var friendRequestsContainer = await _cosmosDbService.GetOrCreateContainerAsync("FriendRequests", "/id");
            var friendRequest = await _cosmosDbService.GetItemAsync<FriendRequest>(friendRequestsContainer, id.ToString());

            if (friendRequest == null)
            {
                return NotFound();
            }

            return Ok(friendRequest);
        }

        [HttpPost]
        public async Task<ActionResult<FriendRequest>> CreateFriendRequest([FromBody] FriendRequest friendRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            friendRequest.Id = Guid.NewGuid();
            var friendRequestsContainer = await _cosmosDbService.GetOrCreateContainerAsync("FriendRequests", "/id");
            await _cosmosDbService.AddItemAsync(friendRequestsContainer, friendRequest);
            return CreatedAtAction(nameof(GetFriendRequest), new { id = friendRequest.Id }, friendRequest);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFriendRequest(Guid id, [FromBody] FriendRequest friendRequest)
        {
            if (!ModelState.IsValid || id != friendRequest.Id)
            {
                return BadRequest(ModelState);
            }

            var friendRequestsContainer = await _cosmosDbService.GetOrCreateContainerAsync("FriendRequests", "/id");
            var existingFriendRequest = await _cosmosDbService.GetItemAsync<FriendRequest>(friendRequestsContainer, id.ToString());

            if (existingFriendRequest == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(friendRequestsContainer, id.ToString(), friendRequest);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFriendRequest(Guid id)
        {
            var friendRequestsContainer = await _cosmosDbService.GetOrCreateContainerAsync("FriendRequests", "/id");
            var existingFriendRequest = await _cosmosDbService.GetItemAsync<FriendRequest>(friendRequestsContainer, id.ToString());

            if (existingFriendRequest == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<FriendRequest>(friendRequestsContainer, id.ToString());
            return NoContent();
        }
    }
}
