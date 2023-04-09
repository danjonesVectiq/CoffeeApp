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
    public class CommentsController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentsController(ICosmosDbRepository cosmosDbRepository)
        {
            _commentService = new CommentService(cosmosDbRepository);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAllComments()
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(Guid id)
        {
            var comment = await _commentService.GetCommentAsync(id);

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
            await _commentService.CreateCommentAsync(comment);
            return CreatedAtAction(nameof(GetComment), new { id = comment.id }, comment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateComment(Guid id, [FromBody] Comment comment)
        {
            if (!ModelState.IsValid || id != comment.id)
            {
                return BadRequest(ModelState);
            }

            var existingComment = await _commentService.GetCommentAsync(id);

            if (existingComment == null)
            {
                return NotFound();
            }

            await _commentService.UpdateCommentAsync(comment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(Guid id)
        {
            var existingComment = await _commentService.GetCommentAsync(id);

            if (existingComment == null)
            {
                return NotFound();
            }

            await _commentService.DeleteCommentAsync(id);
            return NoContent();
        }
    }
}
