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
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly BlobStorageRepository _blobStorageRepository;

        public UsersController(IUserRepository userRepository, BlobStorageRepository blobStorageRepository)
        {
            _userRepository = userRepository;
            _blobStorageRepository = blobStorageRepository;
        }
       

        [HttpPost("{userId}/upload-image")]
        public async Task<IActionResult> UploadUserPicture(Guid userId, [FromForm] IFormFile file)
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
            string blobName = $"{userId}/{timestamp}{fileExtension}";

            var imageUrl = await _blobStorageRepository.UploadImageAsync(blobName, contentType, stream);
            User user = _userRepository.GetAsync(userId).Result;
            user.ImageUrl = imageUrl;
            await _userRepository.UpdateAsync(user);

            return Ok(new { ImageUrl = imageUrl });
        }

        [HttpGet("{id}/preferences")]
        public async Task<ActionResult<UserPreferences>> GetUserPreferences(Guid id)
        {
            var userPreferences = await _userRepository.LoadUserPreferences(id);

            if (userPreferences == null)
            {
                return NotFound();
            }

            return Ok(userPreferences);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _userRepository.GetAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.id = Guid.NewGuid();
            await _userRepository.CreateAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.id }, user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] User user)
        {
            if (!ModelState.IsValid || id != user.id)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userRepository.GetAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            await _userRepository.UpdateAsync(user);
            return NoContent();
        }
    }
}
