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
    public class CommentsController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public CommentsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAllComments()
        {
            var commentsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Comments", "/id");
            var comments = await _cosmosDbService.GetAllItemsAsync<Comment>(commentsContainer);
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(Guid id)
        {
            var commentsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Comments", "/id");
            var comment = await _cosmosDbService.GetItemAsync<Comment>(commentsContainer, id.ToString());

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpPost]
        public async Task<ActionResult<Comment>> CreateComment([FromBody] Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            comment.id = Guid.NewGuid();
            var commentsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Comments", "/id");
            await _cosmosDbService.AddItemAsync(commentsContainer, comment);
            return CreatedAtAction(nameof(GetComment), new { id = comment.id }, comment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateComment(Guid id, [FromBody] Comment comment)
        {
            if (!ModelState.IsValid || id != comment.id)
            {
                return BadRequest(ModelState);
            }

            var commentsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Comments", "/id");
            var existingComment = await _cosmosDbService.GetItemAsync<Comment>(commentsContainer, id.ToString());

            if (existingComment == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateItemAsync(commentsContainer, id.ToString(), comment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(Guid id)
        {
            var commentsContainer = await _cosmosDbService.GetOrCreateContainerAsync("Comments", "/id");
            var existingComment = await _cosmosDbService.GetItemAsync<Comment>(commentsContainer, id.ToString());

            if (existingComment == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteItemAsync<Comment>(commentsContainer, id.ToString());
            return NoContent();
        }
    }
}
