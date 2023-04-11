using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoffeeShopsController : ControllerBase
    {
        private readonly ICoffeeShopService _coffeeShopService;
        private readonly BlobStorageService _blobStorageService;

        public CoffeeShopsController(ICoffeeShopService coffeeShopService, BlobStorageService blobStorageService)
        {
            _coffeeShopService = coffeeShopService;
            _blobStorageService = blobStorageService;
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

            var imageUrl = await _coffeeShopService.UploadImageAsync(coffeeShopId, contentType, stream);
            CoffeeShop coffeeShop = _coffeeShopService.GetAsync(coffeeShopId).Result;
            coffeeShop.ImageUrl = imageUrl;
            await _coffeeShopService.UpdateAsync(coffeeShop);

            return Ok(new { ImageUrl = imageUrl });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoffeeShop>>> GetAllCoffeeShops()
        {
            var coffees = await _coffeeShopService.GetAllAsync();
            return Ok(coffees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoffeeShop>> GetCoffeeShop(Guid id)
        {
            var coffeeShop = await _coffeeShopService.GetAsync(id);

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
            await _coffeeShopService.CreateAsync(coffeeShop);
            return CreatedAtAction(nameof(GetCoffeeShop), new { id = coffeeShop.id }, coffeeShop);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCoffeeShop(Guid id, [FromBody] CoffeeShop coffeeShop)
        {
            if (!ModelState.IsValid || id != coffeeShop.id)
            {
                return BadRequest(ModelState);
            }

            var existingCoffee = await _coffeeShopService.GetAsync(id);

            if (existingCoffee == null)
            {
                return NotFound();
            }

            await _coffeeShopService.UpdateAsync(coffeeShop);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCoffeeShop(Guid id)
        {
            var existingCoffeeShop = await _coffeeShopService.GetAsync(id);

            if (existingCoffeeShop == null)
            {
                return NotFound();
            }

            await _coffeeShopService.DeleteAsync(existingCoffeeShop);
            return NoContent();
        }
    }
}
