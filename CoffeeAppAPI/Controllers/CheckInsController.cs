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
    public class CheckInsController : ControllerBase
    {
        private readonly ICheckInRepository _checkInRepository;

        public CheckInsController(ICheckInRepository checkInRepository)
        {
            _checkInRepository = checkInRepository;
        }
      

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CheckIn>>> GetAllCheckIns()
        {
            var checkIns = await _checkInRepository.GetAllAsync();
            return Ok(checkIns);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CheckIn>> GetCheckIn(Guid id)
        {
            var checkIn = await _checkInRepository.GetAsync(id);

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
            await _checkInRepository.CreateAsync(checkIn);
            return CreatedAtAction(nameof(GetCheckIn), new { id = checkIn.id }, checkIn);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCheckIn(Guid id, [FromBody] CheckIn checkIn)
        {
            if (!ModelState.IsValid || id != checkIn.id)
            {
                return BadRequest(ModelState);
            }

            var existingCheckIn = await _checkInRepository.GetAsync(id);

            if (existingCheckIn == null)
            {
                return NotFound();
            }

            await _checkInRepository.UpdateAsync(checkIn);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCheckIn(Guid id)
        {
            var existingCheckIn = await _checkInRepository.GetAsync(id);

            if (existingCheckIn == null)
            {
                return NotFound();
            }

            await _checkInRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
