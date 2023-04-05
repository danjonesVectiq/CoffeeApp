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
        private readonly UserRepository _userRepository;

        public UsersController(ICosmosDbService cosmosDbService)
        {
            _userRepository = new UserRepository(cosmosDbService);
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
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _userRepository.GetUserAsync(id);

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
            await _userRepository.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.id }, user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] User user)
        {
            if (!ModelState.IsValid || id != user.id)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userRepository.GetUserAsync(id.ToString());

            if (existingUser == null)
            {
                return NotFound();
            }

            await _userRepository.UpdateUserAsync(user);
            return NoContent();
        }
    }
}
