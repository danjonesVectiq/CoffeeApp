using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface ICoffeeShopRepository : IRepository<CoffeeShop>
    {
       // Task DeleteImageForCoffeeShopAsync(Guid id, string ImageUrl);
        Task DeleteAsync(CoffeeShop coffeeShop);
    }

    public class CoffeeShopRepository : CosmosDbRepository<CoffeeShop>, ICoffeeShopRepository
    {

        private readonly IBlobStorageRepository _blobStorageRepository;
        public CoffeeShopRepository(ICosmosDbService cosmosDbService, IBlobStorageRepository blobStorageRepository)
            : base(cosmosDbService, "Coffee", "/id", "CoffeeShop")
        {
            _blobStorageRepository = blobStorageRepository;
        }

        public async Task DeleteAsync(CoffeeShop coffeeShop)
        {
            if (coffeeShop != null && !string.IsNullOrEmpty(coffeeShop.ImageUrl))
            {
                await _blobStorageRepository.DeleteImageAsync(coffeeShop.id, coffeeShop.ImageUrl);
            }
            await base.DeleteAsync(coffeeShop.id);
        }

    }
}
 