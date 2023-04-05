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
        private readonly IReviewService _reviewService;

        public ReviewRepository(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _reviewService.GetAllItemsAsync<Review>(await _reviewService.GetOrCreateContainerAsync("Reviews", "/id"));
        }

        public async Task<Review> GetReviewAsync(Guid id)
        {
            return await _reviewService.GetItemAsync<Review>(await _reviewService.GetOrCreateContainerAsync("Reviews", "/id"), id.ToString());
        }

        public async Task CreateReviewAsync(Review review)
        {
            await _reviewService.AddItemAsync(await _reviewService.GetOrCreateContainerAsync("Reviews", "/id"), review);
        }

        public async Task UpdateReviewAsync(Review review)
        {
            await _reviewService.UpdateItemAsync(await _reviewService.GetOrCreateContainerAsync("Reviews", "/id"), review.id.ToString(), review);
        }

        public async Task DeleteReviewAsync(Guid id)
        {
            await _reviewService.DeleteItemAsync<Review>(await _reviewService.GetOrCreateContainerAsync("Reviews", "/id"), id.ToString());
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId)
        {
            return await _reviewService.GetReviewsByUserIdAsync(userId);
        }
    }
}
