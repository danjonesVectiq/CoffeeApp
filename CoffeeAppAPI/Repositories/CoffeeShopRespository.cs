using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface ICoffeeShopRepository : IRepository<CoffeeShop>
    {
        // Add any CoffeeShop-specific methods here, if needed
    }

    public class CoffeeShopRepository : CosmosDbRepository<CoffeeShop>, ICoffeeShopRepository
    {
        public CoffeeShopRepository(ICosmosDbService cosmosDbService)
            : base(cosmosDbService, "Coffee", "/id", "CoffeeShop")
        {
        }
    }
}
