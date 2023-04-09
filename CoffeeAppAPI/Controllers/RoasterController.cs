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
    public class RoastersController : ControllerBase
    {
        private readonly RoasterService _roasterService;

        public RoastersController(ICosmosDbRepository cosmosDbRepository)
        {
            _roasterService = new RoasterService(cosmosDbRepository);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Roaster>>> GetAllRoasters()
        {
            var roasters = await _roasterService.GetAllAsync();
            return Ok(roasters);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Roaster>> GetRoaster(Guid id)
        {
            var roaster = await _roasterService.GetAsync(id);

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
            await _roasterService.CreateAsync(roaster);
            return CreatedAtAction(nameof(GetRoaster), new { id = roaster.id }, roaster);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRoaster(Guid id, [FromBody] Roaster roaster)
        {
            if (!ModelState.IsValid || id != roaster.id)
            {
                return BadRequest(ModelState);
            }

            var existingRoaster = await _roasterService.GetAsync(id);

            if (existingRoaster == null)
            {
                return NotFound();
            }

            await _roasterService.UpdateAsync(roaster);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRoaster(Guid id)
        {
            var existingRoaster = await _roasterService.GetAsync(id);

            if (existingRoaster == null)
            {
                return NotFound();
            }

            await _roasterService.DeleteAsync(id);
            return NoContent();
        }
    }
}
