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
            var roasters = await _roasterRepository.GetAllAsync();
            return Ok(roasters);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Roaster>> GetRoaster(Guid id)
        {
            var roaster = await _roasterRepository.GetAsync(id);

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
            await _roasterRepository.CreateAsync(roaster);
            return CreatedAtAction(nameof(GetRoaster), new { id = roaster.id }, roaster);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRoaster(Guid id, [FromBody] Roaster roaster)
        {
            if (!ModelState.IsValid || id != roaster.id)
            {
                return BadRequest(ModelState);
            }

            var existingRoaster = await _roasterRepository.GetAsync(id);

            if (existingRoaster == null)
            {
                return NotFound();
            }

            await _roasterRepository.UpdateAsync(roaster);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRoaster(Guid id)
        {
            var existingRoaster = await _roasterRepository.GetAsync(id);

            if (existingRoaster == null)
            {
                return NotFound();
            }

            await _roasterRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
