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
    public class CommentsController : ControllerBase
    {
        private readonly CommentRepository _commentRepository;

        public CommentsController(ICosmosDbService cosmosDbService)
        {
            _commentRepository = new CommentRepository(cosmosDbService);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetAllComments()
        {
            var comments = await _commentRepository.GetAllCommentsAsync();
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(Guid id)
        {
            var comment = await _commentRepository.GetCommentAsync(id);

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
            await _commentRepository.CreateCommentAsync(comment);
            return CreatedAtAction(nameof(GetComment), new { id = comment.id }, comment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateComment(Guid id, [FromBody] Comment comment)
        {
            if (!ModelState.IsValid || id != comment.id)
            {
                return BadRequest(ModelState);
            }

            var existingComment = await _commentRepository.GetCommentAsync(id);

            if (existingComment == null)
            {
                return NotFound();
            }

            await _commentRepository.UpdateCommentAsync(comment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(Guid id)
        {
            var existingComment = await _commentRepository.GetCommentAsync(id);

            if (existingComment == null)
            {
                return NotFound();
            }

            await _commentRepository.DeleteCommentAsync(id);
            return NoContent();
        }
    }
}
