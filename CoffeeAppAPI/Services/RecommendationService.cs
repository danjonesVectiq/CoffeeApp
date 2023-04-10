using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{


    public interface IRecommendationService
    {
        Task AddRecommendationToUser(Guid userId, Recommendation recommendation);
        Task UpdateUserRecommendation(Guid userId, Recommendation recommendation);
        Task SaveUserRecommendations(Guid id, List<Recommendation> recommendations);
        Task<List<Recommendation>> GetRecommendationsForUser(Guid userId);
        Task UpdateAllUserRecommendations(int numberOfRecommendations);
        Task DeleteUserRecommendation(Guid userId, Guid recommendationId);
       

    }

    public class RecommendationService : IRecommendationService
    {
        private readonly ICoffeeService _coffeeService;
        private readonly ICoffeeScoringService _coffeeScoringService;
        private readonly IUserService _userService;

        public RecommendationService(ICoffeeScoringService coffeeScoringService, IUserService userService, ICoffeeService coffeeService)

        {
            _coffeeScoringService = coffeeScoringService;
            _userService = userService;
            _coffeeService = coffeeService;
        }

        public async Task<List<Recommendation>> GetRecommendationsForUser(Guid userId)
        {
            var user = await _userService.GetAsync(userId);
            if (user != null)
            {
                return user.Recommendations;
            }
            else
            {
                // User not found
                throw new InvalidOperationException("User not found.");
            }
        }
        public async Task AddRecommendationToUser(Guid userId, Recommendation recommendation)
        {
            var user = await _userService.GetAsync(userId);
            if (user != null)
            {
                recommendation.id = Guid.NewGuid();
                user.Recommendations.Add(recommendation);
                await _userService.UpdateAsync(user);
            }
            else
            {
                // User not found
                throw new InvalidOperationException("User not found.");
            }
        }
        public async Task SaveUserRecommendations(Guid userId, List<Recommendation> recommendations)
        {
            var user = await _userService.GetAsync(userId);
            if (user != null)
            {
                user.Recommendations = recommendations;
                await _userService.UpdateAsync(user);
            }
        }

        public async Task UpdateAllUserRecommendations(int numberOfRecommendations)
        {
            var allUsers = await _userService.GetAllAsync();
            var allCoffees = (await _coffeeService.GetAllAsync()).ToList();

            foreach (var user in allUsers)
            {
                var recommendedCoffeeIds =_coffeeScoringService.GenerateRecommendations(user, allCoffees, numberOfRecommendations);

                var recommendations = recommendedCoffeeIds.Select(coffeeId => new Recommendation
                {
                    id = Guid.NewGuid(),
                    CoffeeId = coffeeId
                }).ToList();

                await SaveUserRecommendations(user.id, recommendations);
            }
        }

        public async Task UpdateUserRecommendation(Guid userId, Recommendation recommendation)
        {
            var user = await _userService.GetAsync(userId);
            if (user != null)
            {
                var existingRecommendation = user.Recommendations.FirstOrDefault(r => r.id == recommendation.id);

                if (existingRecommendation != null)
                {
                    // Update the recommendation in the user's Recommendations list
                    int index = user.Recommendations.IndexOf(existingRecommendation);
                    user.Recommendations[index] = recommendation;
                    await _userService.UpdateAsync(user);
                }
                else
                {
                    // Recommendation not found in user's Recommendations list
                    throw new InvalidOperationException("Recommendation not found in user's Recommendations list.");
                }
            }
            else
            {
                // User not found
                throw new InvalidOperationException("User not found.");
            }
        }
        public async Task DeleteUserRecommendation(Guid userId, Guid recommendationId)
        {
            var user = await _userService.GetAsync(userId);
            if (user != null)
            {
                var existingRecommendation = user.Recommendations.FirstOrDefault(r => r.id == recommendationId);

                if (existingRecommendation != null)
                {
                    // Remove the recommendation from the user's Recommendations list
                    user.Recommendations.Remove(existingRecommendation);
                    await _userService.UpdateAsync(user);
                }
                else
                {
                    // Recommendation not found in user's Recommendations list
                    throw new InvalidOperationException("Recommendation not found in user's Recommendations list.");
                }
            }
            else
            {
                // User not found
                throw new InvalidOperationException("User not found.");
            }
        }
        public async Task CalculateRecommendationsForUser(Guid userId, int numberOfRecommendations)
        {
            var user = await _userService.GetAsync(userId);
            if (user != null)
            {
                var allCoffees = (await _coffeeService.GetAllAsync()).ToList();
                var recommendedCoffeeIds = _coffeeScoringService.GenerateRecommendations(user, allCoffees, numberOfRecommendations);

                var recommendations = recommendedCoffeeIds.Select(coffeeId => new Recommendation
                {
                    id = Guid.NewGuid(),
                    CoffeeId = coffeeId
                }).ToList();

                await SaveUserRecommendations(userId, recommendations);
            }
            else
            {
                // User not found
                throw new InvalidOperationException("User not found.");
            }
        }


    }
}
