using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using CoffeeAppAPI.Services;
using CoffeeAppAPI.Data;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoffeesController : ControllerBase
    {
        private readonly ICoffeeService _coffeeService;
        private readonly BlobStorageService _blobStorageService;

        public CoffeesController(ICoffeeService coffeeService, IBlobStorageRepository blobStorageRepository)
        {
            _coffeeService = coffeeService;
            _blobStorageService = new BlobStorageService(blobStorageRepository);
        }

        [HttpPost("{coffeeId}/upload-image")]

        public async Task<IActionResult> UploadCoffeePicture(Guid coffeeId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file received.");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            string contentType = file.ContentType;

            var imageUrl = await _coffeeService.UploadImageAsync(coffeeId, contentType, stream);
            Coffee coffee = _coffeeService.GetAsync(coffeeId).Result;
            coffee.ImageUrl = imageUrl;
            await _coffeeService.UpdateAsync(coffee);

            return Ok(new { ImageUrl = imageUrl });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coffee>>> GetAllCoffees()
        {
            var coffees = await _coffeeService.GetAllAsync();
            return Ok(coffees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Coffee>> GetCoffee(Guid id)
        {
            var coffee = await _coffeeService.GetAsync(id);

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
            await _coffeeService.CreateAsync(coffee);
            return CreatedAtAction(nameof(GetCoffee), new { id = coffee.id }, coffee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCoffee(Guid id, [FromBody] Coffee coffee)
        {
            if (!ModelState.IsValid || id != coffee.id)
            {
                return BadRequest(ModelState);
            }

            var existingCoffee = await _coffeeService.GetAsync(id);

            if (existingCoffee == null)
            {
                return NotFound();
            }

            await _coffeeService.UpdateAsync(coffee);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCoffee(Guid id)
        {
            var existingCoffee = await _coffeeService.GetAsync(id);

            if (existingCoffee == null)
            {
                return NotFound();
            }

            await _coffeeService.DeleteAsync(id);
            return NoContent();
        }
    }
}
