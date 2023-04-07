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
    public class CoffeeShopsController : ControllerBase
    {
        private readonly CoffeeShopRepository _coffeeShopRepository;
        private readonly BlobStorageRepository _blobStorageRepository;

        public CoffeeShopsController(ICosmosDbService cosmosDbService, BlobStorageService blobStorageService)
        {
            _coffeeShopRepository = new CoffeeShopRepository(cosmosDbService);
            _blobStorageRepository = new BlobStorageRepository(blobStorageService);
        }
         [HttpPost("{coffeeShopId}/upload-image")]
        public async Task<IActionResult> UploadCoffeeShopPicture(Guid coffeeShopId, [FromForm] IFormFile file)
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
            string blobName = $"{coffeeShopId}/{timestamp}{fileExtension}";

            var imageUrl = await _blobStorageRepository.UploadImageAsync(blobName, contentType, stream);
            CoffeeShop coffeeShop = _coffeeShopRepository.GetCoffeeShopAsync(coffeeShopId).Result;
            coffeeShop.ImageUrl = imageUrl;
            await _coffeeShopRepository.UpdateCoffeeShopAsync(coffeeShop);

            return Ok(new { ImageUrl = imageUrl });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoffeeShop>>> GetAllCoffeeShops()
        {
            var coffees = await _coffeeShopRepository.GetAllCoffeeShopsAsync();
            return Ok(coffees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoffeeShop>> GetCoffeeShop(Guid id)
        {
            var coffeeShop = await _coffeeShopRepository.GetCoffeeShopAsync(id);

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
            await _coffeeShopRepository.CreateCoffeeShopAsync(coffeeShop);
            return CreatedAtAction(nameof(GetCoffeeShop), new { id = coffeeShop.id }, coffeeShop);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCoffeeShop(Guid id, [FromBody] CoffeeShop coffeeShop)
        {
            if (!ModelState.IsValid || id != coffeeShop.id)
            {
                return BadRequest(ModelState);
            }

            var existingCoffee = await _coffeeShopRepository.GetCoffeeShopAsync(id);

            if (existingCoffee == null)
            {
                return NotFound();
            }

            await _coffeeShopRepository.UpdateCoffeeShopAsync(coffeeShop);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCoffeeShop(Guid id)
        {
            var existingCoffeeShop = await _coffeeShopRepository.GetCoffeeShopAsync(id);

            if (existingCoffeeShop == null)
            {
                return NotFound();
            }

            await _coffeeShopRepository.DeleteCoffeeShopAsync(id);
            return NoContent();
        }
    }
}
