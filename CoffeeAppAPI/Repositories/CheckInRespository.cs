using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class CheckInRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public CheckInRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetCheckInsContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("CheckIns", "/id");
        }

        public async Task<IEnumerable<CheckIn>> GetAllCheckInsAsync()
        {
            var checkInsContainer = await GetCheckInsContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<CheckIn>(checkInsContainer);
        }

        public async Task<CheckIn> GetCheckInAsync(Guid id)
        {
            var checkInsContainer = await GetCheckInsContainerAsync();
            return await _cosmosDbService.GetItemAsync<CheckIn>(checkInsContainer, id.ToString());
        }

        public async Task CreateCheckInAsync(CheckIn checkIn)
        {
            var checkInsContainer = await GetCheckInsContainerAsync();
            await _cosmosDbService.AddItemAsync(checkInsContainer, checkIn);
        }

        public async Task UpdateCheckInAsync(CheckIn checkIn)
        {
            var checkInsContainer = await GetCheckInsContainerAsync();
            await _cosmosDbService.UpdateItemAsync(checkInsContainer, checkIn.id.ToString(), checkIn);
        }

        public async Task DeleteCheckInAsync(Guid id)
        {
            var checkInsContainer = await GetCheckInsContainerAsync();
            await _cosmosDbService.DeleteItemAsync<CheckIn>(checkInsContainer, id.ToString());
        }
    }
}
