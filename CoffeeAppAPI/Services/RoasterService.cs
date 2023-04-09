using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public interface IRoasterService : IService<Roaster>
    {
    }

    public class RoasterService : CosmosDbService<Roaster>, IRoasterService
    {
        public RoasterService(ICosmosDbRepository cosmosDbRepository)
            : base(cosmosDbRepository, "Coffee", "/id", "Roaster")
        {
        }
    }
}
