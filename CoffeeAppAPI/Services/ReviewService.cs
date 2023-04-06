using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using CoffeeAppAPI.Models;

namespace CoffeeAppAPI.Services
{
    public interface IReviewService : ICosmosDbService
    {
        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId);
        
    }

    public class ReviewService : CosmosDbService, IReviewService
    {
        public ReviewService(IConfiguration configuration) : base(configuration)
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