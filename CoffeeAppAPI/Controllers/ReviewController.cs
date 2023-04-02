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
        private readonly ReviewRepository _ReviewRepository;

        public ReviewsController(IReviewService reviewService)
        {
            _ReviewRepository = new ReviewRepository(reviewService);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllReviews()
        {
            var reviews = await _ReviewRepository.GetAllReviewsAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(Guid id)
        {
            var review = await _ReviewRepository.GetReviewAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
        }

        private async Task<(double userAverageRating, double userRatingRange)> GetUserRatingInfo(Guid userId)
        {
            // Retrieve all Review objects related to the userId
            var userReviews = await _ReviewRepository.GetReviewsByUserIdAsync(userId);

            // Calculate the user's average rating
            double userAverageRating = userReviews.Average(r => r.Rating);

            // Calculate the user's rating range
            double userMinRating = userReviews.Min(r => r.Rating);
            double userMaxRating = userReviews.Max(r => r.Rating);
            double userRatingRange = userMaxRating - userMinRating;

            return (userAverageRating, userRatingRange);
        }

        [HttpPost]
        public async Task<ActionResult<Review>> CreateReview([FromBody] Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            review.id = Guid.NewGuid();

            // Retrieve all existing review objects related to the userId
            var userReviews = await _ReviewRepository.GetReviewsByUserIdAsync(review.UserId);

            // Calculate the normalized rating for the new review
            review.NormalizedRating = CalculateNormalizedRating(userReviews, review);

            // Update the normalized ratings for all previous reviews
            await UpdateNormalizedRatingsForUser(userReviews);

            await _ReviewRepository.CreateReviewAsync(review);
            return CreatedAtAction(nameof(GetReview), new { id = review.id }, review);
        }

        private double CalculateNormalizedRating(IEnumerable<Review> userReviews, Review currentReview)
        {
            if (userReviews.Count() == 0)
            {
                return 0;
            }

            double userAverageRating = userReviews.Average(r => r.Rating);
            double userMinRating = userReviews.Min(r => r.Rating);
            double userMaxRating = userReviews.Max(r => r.Rating);
            double userRatingRange = userMaxRating - userMinRating;

            if (userRatingRange == 0)
            {
                return 0;
            }

            return (currentReview.Rating - userAverageRating) / userRatingRange;
        }


        private async Task UpdateNormalizedRatingsForUser(IEnumerable<Review> userReviews)
        {
            foreach (var review in userReviews)
            {
                review.NormalizedRating = CalculateNormalizedRating(userReviews, review);
                await _ReviewRepository.UpdateReviewAsync(review);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateReview(Guid id, [FromBody] Review review)
        {
            if (!ModelState.IsValid || id != review.id)
            {
                return BadRequest(ModelState);
            }

            var existingReview = await _ReviewRepository.GetReviewAsync(id);

            if (existingReview == null)
            {
                return NotFound();
            }

            await _ReviewRepository.UpdateReviewAsync(review);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReview(Guid id)
        {
            var existingReview = await _ReviewRepository.GetReviewAsync(id);

            if (existingReview == null)
            {
                return NotFound();
            }

            await _ReviewRepository.DeleteReviewAsync(id);
            return NoContent();
        }
    }
}
