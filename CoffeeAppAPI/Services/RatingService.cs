using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public interface IRatingService<T> : IService<T> where T : Rating
    {
    }

    public class RatingService<T> : CosmosDbService<T>, IRatingService<T> where T : Rating
    {
        public RatingService(ICosmosDbRepository cosmosDbRepository, string entityType)
            : base(cosmosDbRepository, "Interaction", "/id", entityType)
        {
        }

        public static double CalculateAverageRating(User user, List<T> allRatings)
        {
            var userRatings = allRatings.Where(r => r.UserId == user.id).ToList();
            if (userRatings.Count() == 0) return 0;

            double sum = 0;
            foreach (var rating in userRatings)
            {
                sum += rating.RatingValue;
            }
            return sum / userRatings.Count;
        }
    }

    // CoffeeRatingService
    public class CoffeeRatingService : RatingService<CoffeeRating>
    {
        public CoffeeRatingService(ICosmosDbRepository cosmosDbRepository)
            : base(cosmosDbRepository, "CoffeeRating")
        {
        }
    }

    // CoffeeShopRatingService
    public class CoffeeShopRatingService : RatingService<CoffeeShopRating>
    {
        public CoffeeShopRatingService(ICosmosDbRepository cosmosDbRepository)
            : base(cosmosDbRepository, "CoffeeShopRating")
        {
        }
    }

    // RecipeRatingService
    public class RecipeRatingService : RatingService<RecipeRating>
    {
        public RecipeRatingService(ICosmosDbRepository cosmosDbRepository)
            : base(cosmosDbRepository, "RecipeRating")
        {
        }
    }
}