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
    public class CoffeesController : ControllerBase
    {
        private readonly CosmosDbService _cosmosDbService;

        public CoffeesController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coffee>>> GetAllCoffees()
        {
            var coffeesContainer = await _cosmosDbService.GetOrCreateContainerAsync("Coffees", "/id");
            var coffees = await _cosmosDbService.GetAllItemsAsync<Coffee>(coffeesContainer);
            return Ok(coffees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Coffee>> GetCoffee(Guid id)
        {
            var coffeesContainer = await _cosmosDbService.GetOrCreateContainerAsync("Coffees", "/id");
            var coffee = await _cosmosDbService.GetItemAsync<Coffee>(coffeesContainer, id.ToString());

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
            var coffeesContainer = await _cosmosDbService.GetOrCreateContainerAsync("Coffees", "/id");
            await _cosmosDbService.AddItemAsync(coffeesContainer, coffee);
            return CreatedAtAction(nameof(GetCoffee), new { id = coffee.id }, coffee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCoffee(Guid id, [FromBody] Coffee coffee)
        {
            if (!ModelState.IsValid || id != coffee.id)
            {
                return BadRequest(ModelState);
            }

            var coffeesContainer = await _cosmosDbService.GetOrCreateContainerAsync("Coffees", "/id");
            var existingCoffee = await _cosmosDbService.GetItemAsync<Coffee>(coffeesContainer, id.ToString());

            if (existingCoffee == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(coffeesContainer, id.ToString(), coffee);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCoffee(Guid id)
        {
            var coffeesContainer = await _cosmosDbService.GetOrCreateContainerAsync("Coffees", "/id");
            var existingCoffee = await _cosmosDbService.GetItemAsync<Coffee>(coffeesContainer, id.ToString());

            if (existingCoffee == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<Coffee>(coffeesContainer, id.ToString());
            return NoContent();
        }
    }
}
