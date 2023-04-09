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
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipes()
        {
            var recipes = await _recipeService.GetAllAsync();
            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(Guid id)
        {
            var recipe = await _recipeService.GetAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }

        [HttpGet("coffee/{coffeeId}")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipesByCoffeeId(Guid coffeeId)
        {
            var recipes = await _recipeService.GetRecipesByCoffeeIdAsync(coffeeId);
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
            await _recipeService.CreateAsync(recipe);
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.id }, recipe);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRecipe(Guid id, [FromBody] Recipe recipe)
        {
            if (!ModelState.IsValid || id != recipe.id)
            {
                return BadRequest(ModelState);
            }

            var existingRecipe = await _recipeService.GetAsync(id);

            if (existingRecipe == null)
            {
                return NotFound();
            }

            await _recipeService.UpdateAsync(recipe);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRecipe(Guid id)
        {
            var existingRecipe = await _recipeService.GetAsync(id);

            if (existingRecipe == null)
            {
                return NotFound();
            }

            await _recipeService.DeleteAsync(id);
            return NoContent();
        }
    }
}
