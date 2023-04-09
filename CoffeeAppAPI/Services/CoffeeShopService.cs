using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public interface ICoffeeShopService : IService<CoffeeShop>
    {
        // Task DeleteImageForCoffeeShopAsync(Guid id, string ImageUrl);
        Task DeleteAsync(CoffeeShop coffeeShop);
        Task<string> UploadImageAsync(Guid id, string contentType, Stream imageStream);

    }

    public class CoffeeShopService : CosmosDbService<CoffeeShop>, ICoffeeShopService
    {

        private readonly IBlobStorageService _blobStorageService;
        public CoffeeShopService(ICosmosDbRepository cosmosDbRepository, IBlobStorageService blobStorageService)
            : base(cosmosDbRepository, "Coffee", "/id", "CoffeeShop")
        {
            _blobStorageService = blobStorageService;
        }

        public async Task DeleteAsync(CoffeeShop coffeeShop)
        {
            if (coffeeShop != null && !string.IsNullOrEmpty(coffeeShop.ImageUrl))
            {
                await _blobStorageService.DeleteImageAsync(coffeeShop.id, coffeeShop.ImageUrl);
            }
            await base.DeleteAsync(coffeeShop.id);
        }

        public async Task<string> UploadImageAsync(Guid id, string contentType, Stream imageStream)
        {
            // Create the blob name using coffeeId and a timestamp (or a GUID).
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string fileExtension = Helpers.BlobStorageHelpers.GetFileExtensionFromContentType(contentType);
            string blobName = $"{id}/{timestamp}{fileExtension}";
            return await _blobStorageService.UploadImageAsync(blobName, contentType, imageStream);
        }

    }
}
