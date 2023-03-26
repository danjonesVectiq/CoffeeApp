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
    public class ReviewLikesController : ControllerBase
    {
        private readonly ReviewLikeRepository _reviewLikeRepository;

        public ReviewLikesController(ICosmosDbService cosmosDbService)
        {
            _reviewLikeRepository = new ReviewLikeRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewLike>>> GetAllReviewLikes()
        {
            var reviewLikes = await _reviewLikeRepository.GetAllReviewLikesAsync();
            return Ok(reviewLikes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewLike>> GetReviewLike(Guid id)
        {
            var reviewLike = await _reviewLikeRepository.GetReviewLikeAsync(id);

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
            await _reviewLikeRepository.CreateReviewLikeAsync(reviewLike);
            return CreatedAtAction(nameof(GetReviewLike), new { id = reviewLike.id }, reviewLike);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateReviewLike(Guid id, [FromBody] ReviewLike reviewLike)
        {
            if (!ModelState.IsValid || id != reviewLike.id)
            {
                return BadRequest(ModelState);
            }

            var existingReviewLike = await _reviewLikeRepository.GetReviewLikeAsync(id);

            if (existingReviewLike == null)
            {
                return NotFound();
            }

            await _reviewLikeRepository.UpdateReviewLikeAsync(reviewLike);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReviewLike(Guid id)
        {
            var existingReviewLike = await _reviewLikeRepository.GetReviewLikeAsync(id);

            if (existingReviewLike == null)
            {
                return NotFound();
            }

            await _reviewLikeRepository.DeleteReviewLikeAsync(id);
            return NoContent();
        }
    }
}
