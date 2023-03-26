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
    public class CoffeeListsController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public CoffeeListsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoffeeList>>> GetAllCoffeeLists()
        {
            var coffeeListsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeLists", "/id");
            var coffeeLists = await _cosmosDbService.GetAllItemsAsync<CoffeeList>(coffeeListsContainer);
            return Ok(coffeeLists);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoffeeList>> GetCoffeeList(Guid id)
        {
            var coffeeListsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeLists", "/id");
            var coffeeList = await _cosmosDbService.GetItemAsync<CoffeeList>(coffeeListsContainer, id.ToString());

            if (coffeeList == null)
            {
                return NotFound();
            }

            return Ok(coffeeList);
        }

        [HttpPost]
        public async Task<ActionResult<CoffeeList>> CreateCoffeeList([FromBody] CoffeeList coffeeList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            coffeeList.id = Guid.NewGuid();
            var coffeeListsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeLists", "/id");
            await _cosmosDbService.AddItemAsync(coffeeListsContainer, coffeeList);
            return CreatedAtAction(nameof(GetCoffeeList), new { id = coffeeList.id }, coffeeList);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCoffeeList(Guid id, [FromBody] CoffeeList coffeeList)
        {
            if (!ModelState.IsValid || id != coffeeList.id)
            {
                return BadRequest(ModelState);
            }

            var coffeeListsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeLists", "/id");
            var existingCoffeeList = await _cosmosDbService.GetItemAsync<CoffeeList>(coffeeListsContainer, id.ToString());

            if (existingCoffeeList == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(coffeeListsContainer, id.ToString(), coffeeList);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCoffeeList(Guid id)
        {
            var coffeeListsContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeLists", "/id");
            var existingCoffeeList = await _cosmosDbService.GetItemAsync<CoffeeList>(coffeeListsContainer, id.ToString());

            if (existingCoffeeList == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<CoffeeList>(coffeeListsContainer, id.ToString());
            return NoContent();
        }
    }
}
