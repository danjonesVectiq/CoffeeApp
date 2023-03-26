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
    public class RecommendationsController : ControllerBase
    {
        private readonly RecommendationRepository _recommendationRepository;

        public RecommendationsController(ICosmosDbService cosmosDbService)
        {
            _recommendationRepository = new RecommendationRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recommendation>>> GetAllRecommendations()
        {
            var recommendations = await _recommendationRepository.GetAllRecommendationsAsync();
            return Ok(recommendations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recommendation>> GetRecommendation(Guid id)
        {
            var recommendation = await _recommendationRepository.GetRecommendationAsync(id);

            if (recommendation == null)
            {
                return NotFound();
            }

            return Ok(recommendation);
        }

        [HttpPost]
        public async Task<ActionResult<Recommendation>> CreateRecommendation([FromBody] Recommendation recommendation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            recommendation.id = Guid.NewGuid();
            await _recommendationRepository.CreateRecommendationAsync(recommendation);
            return CreatedAtAction(nameof(GetRecommendation), new { id = recommendation.id }, recommendation);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRecommendation(Guid id, [FromBody] Recommendation recommendation)
        {
            if (!ModelState.IsValid || id != recommendation.id)
            {
                return BadRequest(ModelState);
            }

            var existingRecommendation = await _recommendationRepository.GetRecommendationAsync(id);

            if (existingRecommendation == null)
            {
                return NotFound();
            }

            await _recommendationRepository.UpdateRecommendationAsync(recommendation);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRecommendation(Guid id)
        {
            var existingRecommendation = await _recommendationRepository.GetRecommendationAsync(id);

            if (existingRecommendation == null)
            {
                return NotFound();
            }

            await _recommendationRepository.DeleteRecommendationAsync(id);
            return NoContent();
        }
    }
}
