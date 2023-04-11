using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public interface ICoffeeService : IService<Coffee> 
    {
       // Task DeleteImageForCoffeeShopAsync(Guid id, string ImageUrl);
        Task DeleteAsync(Coffee coffee);
        Task<string> UploadImageAsync(Guid id, string contentType, Stream imageStream);
    }

    public class CoffeeService : CosmosDbService<Coffee>, ICoffeeService 
    {

        private readonly IBlobStorageService _blobStorageService;
        public CoffeeService(ICosmosDbRepository cosmosDbRepository, IBlobStorageService blobStorageService)
            : base(cosmosDbRepository, "Coffee", "/id", "Coffee")
        {
            _blobStorageService = blobStorageService;
        }


        public async Task DeleteAsync(Coffee coffee)
        {
            if (coffee != null && !string.IsNullOrEmpty(coffee.ImageUrl))
            {
                await _blobStorageService.DeleteImageAsync(coffee.id, coffee.ImageUrl);
            }
            await base.DeleteAsync(coffee.id);
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