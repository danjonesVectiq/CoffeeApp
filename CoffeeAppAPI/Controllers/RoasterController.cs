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
    public class RoastersController : ControllerBase
    {
        private readonly RoasterRepository _roasterRepository;

        public RoastersController(ICosmosDbService cosmosDbService)
        {
            _roasterRepository = new RoasterRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Roaster>>> GetAllRoasters()
        {
            var roasters = await _roasterRepository.GetAllRoastersAsync();
            return Ok(roasters);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Roaster>> GetRoaster(Guid id)
        {
            var roaster = await _roasterRepository.GetRoasterAsync(id);

            if (roaster == null)
            {
                return NotFound();
            }

            return Ok(roaster);
        }

        [HttpPost]
        public async Task<ActionResult<Roaster>> CreateRoaster([FromBody] Roaster roaster)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            roaster.id = Guid.NewGuid();
            await _roasterRepository.CreateRoasterAsync(roaster);
            return CreatedAtAction(nameof(GetRoaster), new { id = roaster.id }, roaster);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRoaster(Guid id, [FromBody] Roaster roaster)
        {
            if (!ModelState.IsValid || id != roaster.id)
            {
                return BadRequest(ModelState);
            }

            var existingRoaster = await _roasterRepository.GetRoasterAsync(id);

            if (existingRoaster == null)
            {
                return NotFound();
            }

            await _roasterRepository.UpdateRoasterAsync(roaster);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRoaster(Guid id)
        {
            var existingRoaster = await _roasterRepository.GetRoasterAsync(id);

            if (existingRoaster == null)
            {
                return NotFound();
            }

            await _roasterRepository.DeleteRoasterAsync(id);
            return NoContent();
        }
    }
}
