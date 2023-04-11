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
    public class CoffeeRatingsController : ControllerBase
    {
        private readonly CoffeeRatingService _ratingService;

        public CoffeeRatingsController(CoffeeRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoffeeRating>>> GetAllRatings()
        {
            var ratings = await _ratingService.GetAllAsync();
            return Ok(ratings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoffeeRating>> GetRating(Guid id)
        {
            var rating = await _ratingService.GetAsync(id);

            if (rating == null)
            {
                return NotFound();
            }

            return Ok(rating);
        }

        [HttpPost]
        public async Task<ActionResult<CoffeeRating>> CreateRating([FromBody] CoffeeRating rating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            rating.id = Guid.NewGuid();
            await _ratingService.CreateAsync(rating);
            return CreatedAtAction(nameof(GetRating), new { id = rating.id }, rating);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRating(Guid id, [FromBody] CoffeeRating rating)
        {
            if (!ModelState.IsValid || id != rating.id)
            {
                return BadRequest(ModelState);
            }

            var existingRating = await _ratingService.GetAsync(id);

            if (existingRating == null)
            {
                return NotFound();
            }

            await _ratingService.UpdateAsync(rating);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRating(Guid id)
        {
            var existingRating = await _ratingService.GetAsync(id);

            if (existingRating == null)
            {
                return NotFound();
            }

            await _ratingService.DeleteAsync(id);
            return NoContent();
        }
    }
}
