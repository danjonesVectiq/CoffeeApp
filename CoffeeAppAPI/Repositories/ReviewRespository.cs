using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class ReviewRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public ReviewRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetReviewsContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("Reviews", "/id");
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            var reviewsContainer = await GetReviewsContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<Review>(reviewsContainer);
        }

        public async Task<Review> GetReviewAsync(Guid id)
        {
            var reviewsContainer = await GetReviewsContainerAsync();
            return await _cosmosDbService.GetItemAsync<Review>(reviewsContainer, id.ToString());
        }

        public async Task CreateReviewAsync(Review review)
        {
            var reviewsContainer = await GetReviewsContainerAsync();
            await _cosmosDbService.AddItemAsync(reviewsContainer, review);
        }

        public async Task UpdateReviewAsync(Review review)
        {
            var reviewsContainer = await GetReviewsContainerAsync();
            await _cosmosDbService.UpdateItemAsync(reviewsContainer, review.id.ToString(), review);
        }

        public async Task DeleteReviewAsync(Guid id)
        {
            var reviewsContainer = await GetReviewsContainerAsync();
            await _cosmosDbService.DeleteItemAsync<Review>(reviewsContainer, id.ToString());
        }
    }
}
