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
    public class ReviewLikesController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public ReviewLikesController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewLike>>> GetAllReviewLikes()
        {
            var reviewLikesContainer = await _cosmosDbService.GetOrCreateContainerAsync("ReviewLikes", "/id");
            var reviewLikes = await _cosmosDbService.GetAllItemsAsync<ReviewLike>(reviewLikesContainer);
            return Ok(reviewLikes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewLike>> GetReviewLike(Guid id)
        {
            var reviewLikesContainer = await _cosmosDbService.GetOrCreateContainerAsync("ReviewLikes", "/id");
            var reviewLike = await _cosmosDbService.GetItemAsync<ReviewLike>(reviewLikesContainer, id.ToString());

            if (reviewLike == null)
            {
                return NotFound();
            }

            return Ok(reviewLike);
        }

        [HttpPost]
        public async Task<ActionResult<ReviewLike>> CreateReviewLike([FromBody] ReviewLike reviewLike)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            reviewLike.id = Guid.NewGuid();
            var reviewLikesContainer = await _cosmosDbService.GetOrCreateContainerAsync("ReviewLikes", "/id");
            await _cosmosDbService.AddItemAsync(reviewLikesContainer, reviewLike);
            return CreatedAtAction(nameof(GetReviewLike), new { id = reviewLike.id }, reviewLike);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateReviewLike(Guid id, [FromBody] ReviewLike reviewLike)
        {
            if (!ModelState.IsValid || id != reviewLike.id)
            {
                return BadRequest(ModelState);
            }

            var reviewLikesContainer = await _cosmosDbService.GetOrCreateContainerAsync("ReviewLikes", "/id");
            var existingReviewLike = await _cosmosDbService.GetItemAsync<ReviewLike>(reviewLikesContainer, id.ToString());

            if (existingReviewLike == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(reviewLikesContainer, id.ToString(), reviewLike);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReviewLike(Guid id)
        {
            var reviewLikesContainer = await _cosmosDbService.GetOrCreateContainerAsync("ReviewLikes", "/id");
            var existingReviewLike = await _cosmosDbService.GetItemAsync<ReviewLike>(reviewLikesContainer, id.ToString());

            if (existingReviewLike == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<ReviewLike>(reviewLikesContainer, id.ToString());
            return NoContent();
        }
    }
}
