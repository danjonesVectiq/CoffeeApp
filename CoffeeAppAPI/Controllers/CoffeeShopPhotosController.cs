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
    public class CoffeeShopPhotosController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public CoffeeShopPhotosController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoffeeShopPhoto>>> GetAllCoffeeShopPhotos()
        {
            var coffeeShopPhotosContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeShopPhotos", "/id");
            var coffeeShopPhotos = await _cosmosDbService.GetAllItemsAsync<CoffeeShopPhoto>(coffeeShopPhotosContainer);
            return Ok(coffeeShopPhotos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoffeeShopPhoto>> GetCoffeeShopPhoto(Guid id)
        {
            var coffeeShopPhotosContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeShopPhotos", "/id");
            var coffeeShopPhoto = await _cosmosDbService.GetItemAsync<CoffeeShopPhoto>(coffeeShopPhotosContainer, id.ToString());

            if (coffeeShopPhoto == null)
            {
                return NotFound();
            }

            return Ok(coffeeShopPhoto);
        }

        [HttpPost]
        public async Task<ActionResult<CoffeeShopPhoto>> CreateCoffeeShopPhoto([FromBody] CoffeeShopPhoto coffeeShopPhoto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            coffeeShopPhoto.id = Guid.NewGuid();
            var coffeeShopPhotosContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeShopPhotos", "/id");
            await _cosmosDbService.AddItemAsync(coffeeShopPhotosContainer, coffeeShopPhoto);
            return CreatedAtAction(nameof(GetCoffeeShopPhoto), new { id = coffeeShopPhoto.id }, coffeeShopPhoto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCoffeeShopPhoto(Guid id, [FromBody] CoffeeShopPhoto coffeeShopPhoto)
        {
            if (!ModelState.IsValid || id != coffeeShopPhoto.id)
            {
                return BadRequest(ModelState);
            }

            var coffeeShopPhotosContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeShopPhotos", "/id");
            var existingCoffeeShopPhoto = await _cosmosDbService.GetItemAsync<CoffeeShopPhoto>(coffeeShopPhotosContainer, id.ToString());

            if (existingCoffeeShopPhoto == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(coffeeShopPhotosContainer, id.ToString(), coffeeShopPhoto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCoffeeShopPhoto(Guid id)
        {
            var coffeeShopPhotosContainer = await _cosmosDbService.GetOrCreateContainerAsync("CoffeeShopPhotos", "/id");
            var existingCoffeeShopPhoto = await _cosmosDbService.GetItemAsync<CoffeeShopPhoto>(coffeeShopPhotosContainer, id.ToString());

            if (existingCoffeeShopPhoto == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<CoffeeShopPhoto>(coffeeShopPhotosContainer, id.ToString());
            return NoContent();
        }
    }
}
