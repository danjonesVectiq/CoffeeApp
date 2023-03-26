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
    public class RecommendationsController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public RecommendationsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recommendation>>> GetAllRecommendations()
        {
            var recommendationsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Recommendations", "/id");
            var recommendations = await _cosmosDbService.GetAllItemsAsync<Recommendation>(recommendationsContainer);
            return Ok(recommendations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recommendation>> GetRecommendation(Guid id)
        {
            var recommendationsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Recommendations", "/id");
            var recommendation = await _cosmosDbService.GetItemAsync<Recommendation>(recommendationsContainer, id.ToString());

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
            var recommendationsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Recommendations", "/id");
            await _cosmosDbService.AddItemAsync(recommendationsContainer, recommendation);
            return CreatedAtAction(nameof(GetRecommendation), new { id = recommendation.id }, recommendation);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRecommendation(Guid id, [FromBody] Recommendation recommendation)
        {
            if (!ModelState.IsValid || id != recommendation.id)
            {
                return BadRequest(ModelState);
            }

            var recommendationsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Recommendations", "/id");
            var existingRecommendation = await _cosmosDbService.GetItemAsync<Recommendation>(recommendationsContainer, id.ToString());

            if (existingRecommendation == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(recommendationsContainer, id.ToString(), recommendation);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRecommendation(Guid id)
        {
            var recommendationsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Recommendations", "/id");
            var existingRecommendation = await _cosmosDbService.GetItemAsync<Recommendation>(recommendationsContainer, id.ToString());

            if (existingRecommendation == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<Recommendation>(recommendationsContainer, id.ToString());
            return NoContent();
        }
    }
}
