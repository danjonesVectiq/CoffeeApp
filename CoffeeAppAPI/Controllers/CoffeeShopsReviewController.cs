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
    public class CoffeeShopReviewsController : ControllerBase
    {
        private readonly CoffeeShopReviewRepository _coffeeShopReviewRepository;

        public CoffeeShopReviewsController(ICoffeeShopReviewService coffeeShopReviewService)
        {
            _coffeeShopReviewRepository = new CoffeeShopReviewRepository(coffeeShopReviewService);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoffeeShopReview>>> GetAllCoffeeShopReviews()
        {
            var coffeeShopReviews = await _coffeeShopReviewRepository.GetAllCoffeeShopReviewsAsync();
            return Ok(coffeeShopReviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoffeeShopReview>> GetCoffeeShopReview(Guid id)
        {
            var coffeeShopReview = await _coffeeShopReviewRepository.GetCoffeeShopReviewAsync(id);

            if (coffeeShopReview == null)
            {
                return NotFound();
            }

            return Ok(coffeeShopReview);
        }

        private async Task<(double userAverageRating, double userRatingRange)> GetUserRatingInfo(Guid userId)
        {
            // Retrieve all CoffeeShopReview objects related to the userId
            var userReviews = await _coffeeShopReviewRepository.GetReviewsByUserIdAsync(userId);

            // Calculate the user's average rating
            double userAverageRating = userReviews.Average(r => r.Rating);

            // Calculate the user's rating range
            double userMinRating = userReviews.Min(r => r.Rating);
            double userMaxRating = userReviews.Max(r => r.Rating);
            double userRatingRange = userMaxRating - userMinRating;

            return (userAverageRating, userRatingRange);
        }

        [HttpPost]
        public async Task<ActionResult<CoffeeShopReview>> CreateCoffeeShopReview([FromBody] CoffeeShopReview coffeeShopReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            coffeeShopReview.id = Guid.NewGuid();

            // Retrieve all existing CoffeeShopReview objects related to the userId
            var userReviews = await _coffeeShopReviewRepository.GetReviewsByUserIdAsync(coffeeShopReview.UserId);

            // Calculate the normalized rating for the new review
            coffeeShopReview.NormalizedRating = CalculateNormalizedRating(userReviews, coffeeShopReview);

            // Update the normalized ratings for all previous reviews
            await UpdateNormalizedRatingsForUser(userReviews);

            await _coffeeShopReviewRepository.CreateCoffeeShopReviewAsync(coffeeShopReview);
            return CreatedAtAction(nameof(GetCoffeeShopReview), new { id = coffeeShopReview.id }, coffeeShopReview);
        }

        private double CalculateNormalizedRating(IEnumerable<CoffeeShopReview> userReviews, CoffeeShopReview currentReview)
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


        private async Task UpdateNormalizedRatingsForUser(IEnumerable<CoffeeShopReview> userReviews)
        {
            foreach (var review in userReviews)
            {
                review.NormalizedRating = CalculateNormalizedRating(userReviews, review);
                await _coffeeShopReviewRepository.UpdateCoffeeShopReviewAsync(review);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCoffeeShopReview(Guid id, [FromBody] CoffeeShopReview coffeeShopReview)
        {
            if (!ModelState.IsValid || id != coffeeShopReview.id)
            {
                return BadRequest(ModelState);
            }

            var existingCoffeeShopReview = await _coffeeShopReviewRepository.GetCoffeeShopReviewAsync(id);

            if (existingCoffeeShopReview == null)
            {
                return NotFound();
            }

            await _coffeeShopReviewRepository.UpdateCoffeeShopReviewAsync(coffeeShopReview);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCoffeeShopReview(Guid id)
        {
            var existingCoffeeShopReview = await _coffeeShopReviewRepository.GetCoffeeShopReviewAsync(id);

            if (existingCoffeeShopReview == null)
            {
                return NotFound();
            }

            await _coffeeShopReviewRepository.DeleteCoffeeShopReviewAsync(id);
            return NoContent();
        }
    }
}
