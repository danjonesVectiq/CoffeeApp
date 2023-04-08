using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;

        public RecipesController(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipes()
        {
            var recipes = await _recipeRepository.GetAllAsync();
            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(Guid id)
        {
            var recipe = await _recipeRepository.GetAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }

        [HttpGet("coffee/{coffeeId}")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipesByCoffeeId(Guid coffeeId)
        {
            var recipes = await _recipeRepository.GetRecipesByCoffeeIdAsync(coffeeId);
            return Ok(recipes);
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> CreateRecipe([FromBody] Recipe recipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            recipe.id = Guid.NewGuid();
            await _recipeRepository.CreateAsync(recipe);
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.id }, recipe);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRecipe(Guid id, [FromBody] Recipe recipe)
        {
            if (!ModelState.IsValid || id != recipe.id)
            {
                return BadRequest(ModelState);
            }

            var existingRecipe = await _recipeRepository.GetAsync(id);

            if (existingRecipe == null)
            {
                return NotFound();
            }

            await _recipeRepository.UpdateAsync(recipe);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRecipe(Guid id)
        {
            var existingRecipe = await _recipeRepository.GetAsync(id);

            if (existingRecipe == null)
            {
                return NotFound();
            }

            await _recipeRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
