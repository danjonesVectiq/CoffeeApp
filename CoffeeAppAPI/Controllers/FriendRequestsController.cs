/* using System;
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
    public class FriendRequestsController : ControllerBase
    {
        private readonly FriendRequestRepository _friendRequestRepository;

        public FriendRequestsController(ICosmosDbService cosmosDbService)
        {
            _friendRequestRepository = new FriendRequestRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FriendRequest>>> GetAllFriendRequests()
        {
            var friendRequests = await _friendRequestRepository.GetAllFriendRequestsAsync();
            return Ok(friendRequests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FriendRequest>> GetFriendRequest(Guid id)
        {
            var friendRequest = await _friendRequestRepository.GetFriendRequestAsync(id);

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

            friendRequest.id = Guid.NewGuid();
            await _friendRequestRepository.CreateFriendRequestAsync(friendRequest);
            return CreatedAtAction(nameof(GetFriendRequest), new { id = friendRequest.id }, friendRequest);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFriendRequest(Guid id, [FromBody] FriendRequest friendRequest)
        {
            if (!ModelState.IsValid || id != friendRequest.id)
            {
                return BadRequest(ModelState);
            }

            var existingFriendRequest = await _friendRequestRepository.GetFriendRequestAsync(id);

            if (existingFriendRequest == null)
            {
                return NotFound();
            }

            await _friendRequestRepository.UpdateFriendRequestAsync(friendRequest);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFriendRequest(Guid id)
        {
            var existingFriendRequest = await _friendRequestRepository.GetFriendRequestAsync(id);

            if (existingFriendRequest == null)
            {
                return NotFound();
            }

            await _friendRequestRepository.DeleteFriendRequestAsync(id);
            return NoContent();
        }
    }
}
 */