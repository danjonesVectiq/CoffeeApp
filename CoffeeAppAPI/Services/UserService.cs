using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public interface IUserService : IService<User>
    {
        Task DeleteAsync(User user);
        Task<string> UploadImageAsync(Guid coffeeShopId, string contentType, Stream imageStream);

    }
    public class UserService : CosmosDbService<User>, IUserService
    {
        private readonly IBlobStorageService _blobStorageService;
        public UserService(ICosmosDbRepository cosmosDbRepository, IBlobStorageService blobStorageService)
            : base(cosmosDbRepository, "User", "/id", "User")
        {
            _blobStorageService = blobStorageService;
        }
    
        public async Task DeleteAsync(User user)
        {
            if (user != null && !string.IsNullOrEmpty(user.ImageUrl))
            {
                await _blobStorageService.DeleteImageAsync(user.id, user.ImageUrl);
            }
            await base.DeleteAsync(user.id);
        }

        public async Task<string> UploadImageAsync(Guid userId, string contentType, Stream imageStream)
        {
            // Create the blob name using coffeeId and a timestamp (or a GUID).
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string fileExtension = Helpers.BlobStorageHelpers.GetFileExtensionFromContentType(contentType);
            string blobName = $"{userId}/{timestamp}{fileExtension}";

            return await _blobStorageService.UploadImageAsync(blobName, contentType, imageStream);
        }
    }
}
