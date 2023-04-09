using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using CoffeeAppAPI.Services;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
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

            var imageUrl = await _userService.UploadImageAsync(userId, contentType, stream);
            User user = _userService.GetAsync(userId).Result;
            user.ImageUrl = imageUrl;
            await _userService.UpdateAsync(user);

            return Ok(new { ImageUrl = imageUrl });
        }

        [HttpGet("{id}/preferences")]
        public async Task<ActionResult<UserPreferences>> GetUserPreferences(Guid id)
        {
            var userPreferences = await _userService.LoadUserPreferences(id);

            if (userPreferences == null)
            {
                return NotFound();
            }

            return Ok(userPreferences);
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _userService.GetAsync(id);

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
            await _userService.CreateAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.id }, user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] User user)
        {
            if (!ModelState.IsValid || id != user.id)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userService.GetAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            await _userService.UpdateAsync(user);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCoffeeShop(Guid id)
        {
            var existingUser = await _userService.GetAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            await _userService.DeleteAsync(existingUser);
            return NoContent();
        }
    }
}
