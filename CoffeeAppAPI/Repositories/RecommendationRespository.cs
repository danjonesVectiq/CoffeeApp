using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class RecommendationRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public RecommendationRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetRecommendationsContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("Coffee", "/id");
        }

        public async Task<IEnumerable<Recommendation>> GetAllRecommendationsAsync()
        {
            var recommendationsContainer = await GetRecommendationsContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<Recommendation>(recommendationsContainer, "Recommendation");
        }

        public async Task<Recommendation> GetRecommendationAsync(Guid id)
        {
            var recommendationsContainer = await GetRecommendationsContainerAsync();
            return await _cosmosDbService.GetItemAsync<Recommendation>(recommendationsContainer, id.ToString());
        }

        public async Task CreateRecommendationAsync(Recommendation recommendation)
        {
            var recommendationsContainer = await GetRecommendationsContainerAsync();
            await _cosmosDbService.AddItemAsync(recommendationsContainer, recommendation);
        }

        public async Task UpdateRecommendationAsync(Recommendation recommendation)
        {
            var recommendationsContainer = await GetRecommendationsContainerAsync();
            await _cosmosDbService.UpdateItemAsync(recommendationsContainer, recommendation.id.ToString(), recommendation);
        }

        public async Task DeleteRecommendationAsync(Guid id)
        {
            var recommendationsContainer = await GetRecommendationsContainerAsync();
            await _cosmosDbService.DeleteItemAsync<Recommendation>(recommendationsContainer, id.ToString());
        }
    }
}
