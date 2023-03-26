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
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewRepository _reviewRepository;

        public ReviewsController(ICosmosDbService cosmosDbService)
        {
            _reviewRepository = new ReviewRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllReviews()
        {
            var reviews = await _reviewRepository.GetAllReviewsAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(Guid id)
        {
            var review = await _reviewRepository.GetReviewAsync(id);

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
            await _reviewRepository.CreateReviewAsync(review);
            return CreatedAtAction(nameof(GetReview), new { id = review.id }, review);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateReview(Guid id, [FromBody] Review review)
        {
            if (!ModelState.IsValid || id != review.id)
            {
                return BadRequest(ModelState);
            }

            var existingReview = await _reviewRepository.GetReviewAsync(id);

            if (existingReview == null)
            {
                return NotFound();
            }

            await _reviewRepository.UpdateReviewAsync(review);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReview(Guid id)
        {
            var existingReview = await _reviewRepository.GetReviewAsync(id);

            if (existingReview == null)
            {
                return NotFound();
            }

            await _reviewRepository.DeleteReviewAsync(id);
            return NoContent();
        }
    }
}
