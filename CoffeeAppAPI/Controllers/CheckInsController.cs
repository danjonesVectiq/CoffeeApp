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
    public class CheckInsController : ControllerBase
    {
        private readonly CosmosDbService _cosmosDbService;

        public CheckInsController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CheckIn>>> GetAllCheckIns()
        {
            var checkInsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CheckIns", "/checkinId");
            var checkIns = await _cosmosDbService.GetAllItemsAsync<CheckIn>(checkInsContainer);
            return Ok(checkIns);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CheckIn>> GetCheckIn(Guid id)
        {
            var checkInsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CheckIns", "/checkinId");
            var checkIn = await _cosmosDbService.GetItemAsync<CheckIn>(checkInsContainer, id.ToString());

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
            var checkInsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CheckIns", "/checkinId");
            await _cosmosDbService.AddItemAsync(checkInsContainer, checkIn);
            return CreatedAtAction(nameof(GetCheckIn), new { id = checkIn.id }, checkIn);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCheckIn(Guid id, [FromBody] CheckIn checkIn)
        {
            if (!ModelState.IsValid || id != checkIn.id)
            {
                return BadRequest(ModelState);
            }

            var checkInsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CheckIns", "/checkinId");
            var existingCheckIn = await _cosmosDbService.GetItemAsync<CheckIn>(checkInsContainer, id.ToString());

            if (existingCheckIn == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(checkInsContainer, id.ToString(), checkIn);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCheckIn(Guid id)
        {
            var checkInsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CheckIns", "/checkinId");
            var existingCheckIn = await _cosmosDbService.GetItemAsync<CheckIn>(checkInsContainer, id.ToString());

            if (existingCheckIn == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<CheckIn>(checkInsContainer, id.ToString());
            return NoContent();
        }
    }
}
