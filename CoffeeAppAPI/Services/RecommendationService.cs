using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public interface IRecommendationService : IService<Recommendation>
    {
        
    }
    
    public class RecommendationService : CosmosDbService<Recommendation>, IRecommendationService
    {


         public RecommendationService(ICosmosDbRepository cosmosDbRepository)
            : base(cosmosDbRepository, "Coffee", "/id", "Recommendation")
        {
        }

        
    }
}
