using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public interface ICoffeeService : IService<Coffee>
    {
        // Add any Coffee-specific methods here, if needed
    }

    public class CoffeeService : CosmosDbService<Coffee>, ICoffeeService
    {
        public CoffeeService(ICosmosDbRepository cosmosDbRepository)
            : base(cosmosDbRepository, "Coffee", "/id", "Coffee")
        {
        }

        // Implement any Coffee-specific methods here, if needed

      
    }
}