using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class ReviewLikeRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public ReviewLikeRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetReviewLikesContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("ReviewLikes", "/id");
        }

        public async Task<IEnumerable<ReviewLike>> GetAllReviewLikesAsync()
        {
            var reviewLikesContainer = await GetReviewLikesContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<ReviewLike>(reviewLikesContainer);
        }

        public async Task<ReviewLike> GetReviewLikeAsync(Guid id)
        {
            var reviewLikesContainer = await GetReviewLikesContainerAsync();
            return await _cosmosDbService.GetItemAsync<ReviewLike>(reviewLikesContainer, id.ToString());
        }

        public async Task CreateReviewLikeAsync(ReviewLike reviewLike)
        {
            var reviewLikesContainer = await GetReviewLikesContainerAsync();
            await _cosmosDbService.AddItemAsync(reviewLikesContainer, reviewLike);
        }

        public async Task UpdateReviewLikeAsync(ReviewLike reviewLike)
        {
            var reviewLikesContainer = await GetReviewLikesContainerAsync();
            await _cosmosDbService.UpdateItemAsync(reviewLikesContainer, reviewLike.id.ToString(), reviewLike);
        }

        public async Task DeleteReviewLikeAsync(Guid id)
        {
            var reviewLikesContainer = await GetReviewLikesContainerAsync();
            await _cosmosDbService.DeleteItemAsync<ReviewLike>(reviewLikesContainer, id.ToString());
        }
    }
}
