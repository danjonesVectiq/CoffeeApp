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
    public class CoffeesController : ControllerBase
    {
        private readonly CoffeeRepository _coffeeRepository;

        public CoffeesController(ICosmosDbService cosmosDbService)
        {
            _coffeeRepository = new CoffeeRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coffee>>> GetAllCoffees()
        {
            var coffees = await _coffeeRepository.GetAllCoffeesAsync();
            return Ok(coffees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Coffee>> GetCoffee(Guid id)
        {
            var coffee = await _coffeeRepository.GetCoffeeAsync(id);

            if (coffee == null)
            {
                return NotFound();
            }

            return Ok(coffee);
        }

        [HttpPost]
        public async Task<ActionResult<Coffee>> CreateCoffee([FromBody] Coffee coffee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            coffee.id = Guid.NewGuid();
            await _coffeeRepository.CreateCoffeeAsync(coffee);
            return CreatedAtAction(nameof(GetCoffee), new { id = coffee.id }, coffee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCoffee(Guid id, [FromBody] Coffee coffee)
        {
            if (!ModelState.IsValid || id != coffee.id)
            {
                return BadRequest(ModelState);
            }

            var existingCoffee = await _coffeeRepository.GetCoffeeAsync(id);

            if (existingCoffee == null)
            {
                return NotFound();
            }

            await _coffeeRepository.UpdateCoffeeAsync(coffee);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCoffee(Guid id)
        {
            var existingCoffee = await _coffeeRepository.GetCoffeeAsync(id);

            if (existingCoffee == null)
            {
                return NotFound();
            }

            await _coffeeRepository.DeleteCoffeeAsync(id);
            return NoContent();
        }
    }
}
