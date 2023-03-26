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
        private readonly CheckInRepository _checkInRepository;

        public CheckInsController(ICosmosDbService cosmosDbService)
        {
            _checkInRepository = new CheckInRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CheckIn>>> GetAllCheckIns()
        {
            var checkIns = await _checkInRepository.GetAllCheckInsAsync();
            return Ok(checkIns);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CheckIn>> GetCheckIn(Guid id)
        {
            var checkIn = await _checkInRepository.GetCheckInAsync(id);

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
            await _checkInRepository.CreateCheckInAsync(checkIn);
            return CreatedAtAction(nameof(GetCheckIn), new { id = checkIn.id }, checkIn);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCheckIn(Guid id, [FromBody] CheckIn checkIn)
        {
            if (!ModelState.IsValid || id != checkIn.id)
            {
                return BadRequest(ModelState);
            }

            var existingCheckIn = await _checkInRepository.GetCheckInAsync(id);

            if (existingCheckIn == null)
            {
                return NotFound();
            }

            await _checkInRepository.UpdateCheckInAsync(checkIn);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCheckIn(Guid id)
        {
            var existingCheckIn = await _checkInRepository.GetCheckInAsync(id);

            if (existingCheckIn == null)
            {
                return NotFound();
            }

            await _checkInRepository.DeleteCheckInAsync(id);
            return NoContent();
        }
    }
}
