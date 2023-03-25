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
    public class CoffeeShopsController : ControllerBase
    {
        private readonly CosmosDbService _cosmosDbService;

        public CoffeeShopsController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoffeeShop>>> GetAllCoffeeShops()
        {
            var coffeeShopsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeShops", "/id");
            var coffeeShops = await _cosmosDbService.GetAllItemsAsync<CoffeeShop>(coffeeShopsContainer);
            return Ok(coffeeShops);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoffeeShop>> GetCoffeeShop(Guid id)
        {
            var coffeeShopsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeShops", "/id");
            var coffeeShop = await _cosmosDbService.GetItemAsync<CoffeeShop>(coffeeShopsContainer, id.ToString());

            if (coffeeShop == null)
            {
                return NotFound();
            }

            return Ok(coffeeShop);
        }

        [HttpPost]
        public async Task<ActionResult<CoffeeShop>> CreateCoffeeShop([FromBody] CoffeeShop coffeeShop)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            coffeeShop.id = Guid.NewGuid();
            var coffeeShopsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeShops", "/id");
            await _cosmosDbService.AddItemAsync(coffeeShopsContainer, coffeeShop);
            return CreatedAtAction(nameof(GetCoffeeShop), new { id = coffeeShop.id }, coffeeShop);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCoffeeShop(Guid id, [FromBody] CoffeeShop coffeeShop)
        {
            if (!ModelState.IsValid || id != coffeeShop.id)
            {
                return BadRequest(ModelState);
            }

            var coffeeShopsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeShops", "/id");
            var existingCoffeeShop = await _cosmosDbService.GetItemAsync<CoffeeShop>(coffeeShopsContainer, id.ToString());

            if (existingCoffeeShop == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(coffeeShopsContainer, id.ToString(), coffeeShop);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCoffeeShop(Guid id)
        {
            var coffeeShopsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeShops", "/id");
            var existingCoffeeShop = await _cosmosDbService.GetItemAsync<CoffeeShop>(coffeeShopsContainer, id.ToString());

            if (existingCoffeeShop == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<CoffeeShop>(coffeeShopsContainer, id.ToString());
            return NoContent();
        }
    }
}
