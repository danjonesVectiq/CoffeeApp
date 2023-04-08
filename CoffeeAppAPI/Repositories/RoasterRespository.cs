using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface IRoasterRepository : IRepository<Roaster>
    {
    }

    public class RoasterRepository : CosmosDbRepository<Roaster>, IRoasterRepository
    {
        public RoasterRepository(ICosmosDbService cosmosDbService)
            : base(cosmosDbService, "Coffee", "/id", "Roaster")
        {
        }
    }
}
