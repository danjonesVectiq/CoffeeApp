using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class CoffeeShopReviewRepository
    {
        private readonly ICoffeeShopReviewService _coffeeShopReviewService;

        public CoffeeShopReviewRepository(ICoffeeShopReviewService coffeeShopReviewService)
        {
            _coffeeShopReviewService = coffeeShopReviewService;
        }

        public async Task<IEnumerable<CoffeeShopReview>> GetAllCoffeeShopReviewsAsync()
        {
            return await _coffeeShopReviewService.GetAllItemsAsync<CoffeeShopReview>(await _coffeeShopReviewService.GetOrCreateContainerAsync("CoffeeShopReviews", "/id"));
        }

        public async Task<CoffeeShopReview> GetCoffeeShopReviewAsync(Guid id)
        {
            return await _coffeeShopReviewService.GetItemAsync<CoffeeShopReview>(await _coffeeShopReviewService.GetOrCreateContainerAsync("CoffeeShopReviews", "/id"), id.ToString());
        }

        public async Task CreateCoffeeShopReviewAsync(CoffeeShopReview coffeeShopReview)
        {
            await _coffeeShopReviewService.AddItemAsync(await _coffeeShopReviewService.GetOrCreateContainerAsync("CoffeeShopReviews", "/id"), coffeeShopReview);
        }

        public async Task UpdateCoffeeShopReviewAsync(CoffeeShopReview coffeeShopReview)
        {
            await _coffeeShopReviewService.UpdateItemAsync(await _coffeeShopReviewService.GetOrCreateContainerAsync("CoffeeShopReviews", "/id"), coffeeShopReview.id.ToString(), coffeeShopReview);
        }

        public async Task DeleteCoffeeShopReviewAsync(Guid id)
        {
            await _coffeeShopReviewService.DeleteItemAsync<CoffeeShopReview>(await _coffeeShopReviewService.GetOrCreateContainerAsync("CoffeeShopReviews", "/id"), id.ToString());
        }

        public async Task<IEnumerable<CoffeeShopReview>> GetReviewsByUserIdAsync(Guid userId)
        {
            return await _coffeeShopReviewService.GetCoffeeShopReviewsByUserIdAsync(userId);
        }
    }
}
