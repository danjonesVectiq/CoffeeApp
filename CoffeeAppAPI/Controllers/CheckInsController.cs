using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using CoffeeAppAPI.Services;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInsController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInsController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }
      
      [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CheckIn>>> GetUserCheckInHistory(Guid userId)
        {
            var checkIns = await _checkInService.GetUserCheckInsAsync(userId);

            if (checkIns == null)
            {
                return NotFound();
            }

            return Ok(checkIns);
        }

        [HttpGet("coffeeshop/{coffeeShopId}")]
        public async Task<ActionResult<IEnumerable<CheckIn>>> GetCoffeeShopCheckIns(Guid coffeeShopId)
        {
            var checkIns = await _checkInService.GetCoffeeShopCheckInsAsync(coffeeShopId);

            if (checkIns == null)
            {
                return NotFound();
            }

            return Ok(checkIns);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CheckIn>>> GetAllCheckIns()
        {
            var checkIns = await _checkInService.GetAllAsync();
            return Ok(checkIns);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CheckIn>> GetCheckIn(Guid id)
        {
            var checkIn = await _checkInService.GetAsync(id);

            if (checkIn == null)
            {
                return NotFound();
            }

            return Ok(checkIn);
        }

        [HttpPost]
        public async Task<ActionResult<CheckIn>> CreateCheckIn([FromBody] CheckIn checkIn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            checkIn.id = Guid.NewGuid();
            await _checkInService.CreateAsync(checkIn);
            return CreatedAtAction(nameof(GetCheckIn), new { id = checkIn.id }, checkIn);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCheckIn(Guid id, [FromBody] CheckIn checkIn)
        {
            if (!ModelState.IsValid || id != checkIn.id)
            {
                return BadRequest(ModelState);
            }

            var existingCheckIn = await _checkInService.GetAsync(id);

            if (existingCheckIn == null)
            {
                return NotFound();
            }

            await _checkInService.UpdateAsync(checkIn);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCheckIn(Guid id)
        {
            var existingCheckIn = await _checkInService.GetAsync(id);

            if (existingCheckIn == null)
            {
                return NotFound();
            }

            await _checkInService.DeleteAsync(id);
            return NoContent();
        }
    }
}
