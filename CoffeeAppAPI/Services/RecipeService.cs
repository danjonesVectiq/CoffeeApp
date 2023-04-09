using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public interface IRecipeService : IService<Recipe>
    {
        Task<IEnumerable<Recipe>> GetRecipesByCoffeeIdAsync(Guid coffeeId);
    }

    public class RecipeService : CosmosDbService<Recipe>, IRecipeService
    {
        public RecipeService(ICosmosDbRepository cosmosDbRepository)
            : base(cosmosDbRepository, "Coffee", "/id", "Recipe")
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
