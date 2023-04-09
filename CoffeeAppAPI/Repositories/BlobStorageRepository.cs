using Azure.Storage.Blobs;
using CoffeeAppAPI.Services;
using CoffeeAppAPI.Helpers;


namespace CoffeeAppAPI.Repositories
{
    public interface IBlobStorageRepository
    {
        Task<string> UploadImageAsync(string blobName, string contentType, Stream imageStream);
        Task DeleteImageAsync(Guid id, string ImageUrl);
    }
    public class BlobStorageRepository : IBlobStorageRepository
    {
        private readonly IBlobStorageService _blobStorageService;
        public BlobStorageRepository(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        public async Task<string> UploadImageAsync(string blobName, string contentType, Stream imageStream)
        {
            
            return await _blobStorageService.UploadImageAsync(blobName, contentType, imageStream);
        }
         public async Task DeleteImageAsync(Guid id, string ImageUrl)
        {
            if (ImageUrl.StartsWith("https://coffeeappstorage.blob.core.windows.net/coffeeappcontainer/"))
            {
                await _blobStorageService.DeleteImageAsync(id.ToString());
            }
        }

    }
}