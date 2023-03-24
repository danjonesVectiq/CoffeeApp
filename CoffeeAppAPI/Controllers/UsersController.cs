using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeApp.Models;
using CoffeeApp.Services;

namespace CoffeeApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly CosmosDbService _cosmosDbService;

        public UsersController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var usersContainer = await _cosmosDbService.GetOrCreateContainerAsync("Users", "/userId");
            var users = await _cosmosDbService.GetAllItemsAsync<User>(usersContainer);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var usersContainer = await _cosmosDbService.GetOrCreateContainerAsync("Users", "/userId");
            var user = await _cosmosDbService.GetItemAsync<User>(usersContainer, id.ToString());

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

            user.UserId = Guid.NewGuid();
            var usersContainer = await _cosmosDbService.GetOrCreateContainerAsync("Users", "/userId");
            await _cosmosDbService.AddItemAsync(usersContainer, user);
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] User user)
        {
            if (!ModelState.IsValid || id != user.UserId)
            {
                return BadRequest(ModelState);
            }

            var usersContainer = await _cosmosDbService.GetOrCreateContainerAsync("Users", "/userId");
            var existingUser = await _cosmosDbService.GetItemAsync<User>(usersContainer, id.ToString());

            if (existingUser == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(usersContainer, id.ToString(), user);
            return NoContent();
        }

        
    }
}
