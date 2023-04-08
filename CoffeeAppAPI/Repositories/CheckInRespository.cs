using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface ICheckInRepository : IRepository<CheckIn>
    {
        Task<IEnumerable<CheckIn>> GetUserCheckInsAsync(Guid userId);
        Task<IEnumerable<CheckIn>> GetCoffeeShopCheckInsAsync(Guid coffeeShopId);
    }


    public class CheckInRepository : CosmosDbRepository<CheckIn>, ICheckInRepository
    {
        public CheckInRepository(ICosmosDbService cosmosDbService)
            : base(cosmosDbService, "Interaction", "/id", "CheckIn")
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
