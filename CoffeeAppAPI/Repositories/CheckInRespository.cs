using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface ICheckInRepository : IRepository<CheckIn>
    {

    }


    public class CheckInRepository : CosmosDbRepository<CheckIn>, ICheckInRepository
    {
        public CheckInRepository(ICosmosDbService cosmosDbService)
            : base(cosmosDbService, "Interaction", "/id", "CheckIn")
        {
        }
    }
}
