using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using CoffeeAppAPI.Repositories;
using CoffeeAppAPI.Data;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoffeesController : ControllerBase
    {
        private readonly CoffeeRepository _coffeeRepository;
        private readonly BlobStorageRepository _blobStorageRepository;

        public CoffeesController(ICosmosDbService cosmosDbService, IBlobStorageService blobStorageService)
        {
            _coffeeRepository = new CoffeeRepository(cosmosDbService);
            _blobStorageRepository = new BlobStorageRepository(blobStorageService);
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

        

            // Create the blob name using coffeeId and a timestamp (or a GUID).
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string fileExtension = Helpers.BlobStorageHelpers.GetFileExtensionFromContentType(contentType);
            string blobName = $"{coffeeId}/{timestamp}{fileExtension}";

            var imageUrl = await _blobStorageRepository.UploadImageAsync(blobName, contentType, stream);
            Coffee coffee = _coffeeRepository.GetCoffeeAsync(coffeeId).Result;
            coffee.ImageUrl = imageUrl;
            await _coffeeRepository.UpdateCoffeeAsync(coffee);

            return Ok(new { ImageUrl = imageUrl });
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
