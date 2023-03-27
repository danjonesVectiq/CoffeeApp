using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using CoffeeAppAPI.Models;

namespace CoffeeAppAPI.Services
{
    public interface ICoffeeShopReviewService : ICosmosDbService
    {
        Task<IEnumerable<CoffeeShopReview>> GetCoffeeShopReviewsByUserIdAsync(Guid userId);
    }

    public class CoffeeShopReviewService : CosmosDbService, ICoffeeShopReviewService
    {
        public CoffeeShopReviewService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IEnumerable<CoffeeShopReview>> GetCoffeeShopReviewsByUserIdAsync(Guid userId)
        {
            Container container = await GetOrCreateContainerAsync("CoffeeShopReviews", "/id");
            var query = new QueryDefinition($"SELECT * FROM c WHERE c.UserId = '{userId}'");
            var iterator = container.GetItemQueryIterator<CoffeeShopReview>(query);
            List<CoffeeShopReview> results = new List<CoffeeShopReview>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }
    }
}