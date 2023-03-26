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
    public class CoffeeListsController : ControllerBase
    {
        private readonly CoffeeListRepository _coffeeListRepository;

        public CoffeeListsController(ICosmosDbService cosmosDbService)
        {
            _coffeeListRepository = new CoffeeListRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoffeeList>>> GetAllCoffeeLists()
        {
            var coffeeLists = await _coffeeListRepository.GetAllCoffeeListsAsync();
            return Ok(coffeeLists);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoffeeList>> GetCoffeeList(Guid id)
        {
            var coffeeList = await _coffeeListRepository.GetCoffeeListAsync(id);

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
            await _coffeeListRepository.CreateCoffeeListAsync(coffeeList);
            return CreatedAtAction(nameof(GetCoffeeList), new { id = coffeeList.id }, coffeeList);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCoffeeList(Guid id, [FromBody] CoffeeList coffeeList)
        {
            if (!ModelState.IsValid || id != coffeeList.id)
            {
                return BadRequest(ModelState);
            }

            var existingCoffeeList = await _coffeeListRepository.GetCoffeeListAsync(id);

            if (existingCoffeeList == null)
            {
                return NotFound();
            }

            await _coffeeListRepository.UpdateCoffeeListAsync(coffeeList);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCoffeeList(Guid id)
        {
            var existingCoffeeList = await _coffeeListRepository.GetCoffeeListAsync(id);

            if (existingCoffeeList == null)
            {
                return NotFound();
            }

            await _coffeeListRepository.DeleteCoffeeListAsync(id);
            return NoContent();
        }
    }
}
