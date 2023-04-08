using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface IRecommendationRepository : IRepository<Recommendation>
    {
        
    }
    
    public class RecommendationRepository : CosmosDbRepository<Recommendation>, IRecommendationRepository
    {


         public RecommendationRepository(ICosmosDbService cosmosDbService)
            : base(cosmosDbService, "Coffee", "/id", "Recommendation")
        {
        }

        
    }
}
