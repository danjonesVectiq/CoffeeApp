using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface IRecipeRepository : IRepository<Recipe>
    {
        Task<IEnumerable<Recipe>> GetRecipesByCoffeeIdAsync(Guid coffeeId);
    }

    public class RecipeRepository : CosmosDbRepository<Recipe>, IRecipeRepository
    {
        public RecipeRepository(ICosmosDbService cosmosDbService)
            : base(cosmosDbService, "Coffee", "/id", "Recipe")
        {
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByCoffeeIdAsync(Guid coffeeId)
        {
            var query = Container.GetItemLinqQueryable<Recipe>(true)
                .Where(r => r.Type == "Recipe" && r.CoffeeId == coffeeId)
                .ToFeedIterator();

            var recipes = new List<Recipe>();
            while (query.HasMoreResults)
            {
                var resultSet = await query.ReadNextAsync();
                recipes.AddRange(resultSet.Resource);
            }

            return recipes;
        }
    }
}
