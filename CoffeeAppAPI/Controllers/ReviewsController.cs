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
    public class ReviewsController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public ReviewsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllReviews()
        {
            var reviewsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Reviews", "/id");
            var reviews = await _cosmosDbService.GetAllItemsAsync<Review>(reviewsContainer);
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(Guid id)
        {
            var reviewsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Reviews", "/id");
            var review = await _cosmosDbService.GetItemAsync<Review>(reviewsContainer, id.ToString());

            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
        }

        [HttpPost]
        public async Task<ActionResult<Review>> CreateReview([FromBody] Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            review.id = Guid.NewGuid();
            var reviewsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Reviews", "/id");
            await _cosmosDbService.AddItemAsync(reviewsContainer, review);
            return CreatedAtAction(nameof(GetReview), new { id = review.id }, review);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateReview(Guid id, [FromBody] Review review)
        {
            if (!ModelState.IsValid || id != review.id)
            {
                return BadRequest(ModelState);
            }

            var reviewsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Reviews", "/id");
            var existingReview = await _cosmosDbService.GetItemAsync<Review>(reviewsContainer, id.ToString());

            if (existingReview == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(reviewsContainer, id.ToString(), review);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReview(Guid id)
        {
            var reviewsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Reviews", "/id");
            var existingReview = await _cosmosDbService.GetItemAsync<Review>(reviewsContainer, id.ToString());

            if (existingReview == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<Review>(reviewsContainer, id.ToString());
            return NoContent();
        }
    }
}
