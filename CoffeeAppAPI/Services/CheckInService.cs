using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public interface ICheckInService : IService<CheckIn>
    {
        Task<IEnumerable<CheckIn>> GetUserCheckInsAsync(Guid userId);
        Task<IEnumerable<CheckIn>> GetCoffeeShopCheckInsAsync(Guid coffeeShopId);
    }


    public class CheckInService : CosmosDbService<CheckIn>, ICheckInService
    {
        public CheckInService(ICosmosDbRepository cosmosDbRepository)
            : base(cosmosDbRepository, "Interaction", "/id", "CheckIn")
        {
        }

        public async Task<IEnumerable<CheckIn>> GetUserCheckInsAsync(Guid userId)
        {
            var query = Container.GetItemLinqQueryable<CheckIn>(true)
                .Where(r => r.Type == "CheckIn" && r.User.id == userId)
                .ToFeedIterator();

            var checkIns = new List<CheckIn>();
            while (query.HasMoreResults)
            {
                var resultSet = await query.ReadNextAsync();
                checkIns.AddRange(resultSet.Resource);
            }

            return checkIns;
        }

        public async Task<IEnumerable<CheckIn>> GetCoffeeShopCheckInsAsync(Guid coffeeShopId)
        {
            var query = Container.GetItemLinqQueryable<CheckIn>(true)
                .Where(r => r.Type == "CheckIn" && r.CoffeeShop.id == coffeeShopId)
                .ToFeedIterator();

            var checkIns = new List<CheckIn>();
            while (query.HasMoreResults)
            {
                var resultSet = await query.ReadNextAsync();
                checkIns.AddRange(resultSet.Resource);
            }

            return checkIns;
        }
    }
}
