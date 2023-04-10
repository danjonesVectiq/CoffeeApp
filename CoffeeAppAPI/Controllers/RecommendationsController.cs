using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/users/{userId}/[controller]")]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationsController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        // GET: api/users/{userId}/recommendations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recommendation>>> GetRecommendationsForUser(Guid userId)
        {
            var recommendations = await _recommendationService.GetRecommendationsForUser(userId);
            return Ok(recommendations);
        }

        // PUT: api/users/{userId}/recommendations
        [HttpPut]
        public async Task<ActionResult> UpdateRecommendationsForUser(Guid userId, [FromBody] List<Recommendation> recommendations)
        {
            await _recommendationService.SaveUserRecommendations(userId, recommendations);
            return NoContent();
        }

        // POST: api/users/{userId}/recommendations
        [HttpPost]
        public async Task<ActionResult> AddRecommendationToUser(Guid userId, [FromBody] Recommendation recommendation)
        {
            await _recommendationService.AddRecommendationToUser(userId, recommendation);
            return CreatedAtAction(nameof(GetRecommendationsForUser), new { userId = userId }, recommendation);
        }

        // DELETE: api/users/{userId}/recommendations/{recommendationId}
        [HttpDelete("{recommendationId}")]
        public async Task<ActionResult> DeleteRecommendationFromUser(Guid userId, Guid recommendationId)
        {
            await _recommendationService.DeleteUserRecommendation(userId, recommendationId);
            return NoContent();
        }
    }
}
