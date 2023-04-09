using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using CoffeeAppAPI.Models;

namespace CoffeeAppAPI.Repositories
{
    public interface IReviewRepository : ICosmosDbRepository
    {
        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId);
        
    }

    public class ReviewRepository : CosmosDbRepository, IReviewRepository
    {
        public ReviewRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId)
        {
            Container container = await GetOrCreateContainerAsync("Interaction", "/id");
            var query = new QueryDefinition($"SELECT * FROM c WHERE c.UserId = '{userId}'");
            var iterator = container.GetItemQueryIterator<Review>(query);
            List<Review> results = new List<Review>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }
    }
}