using Azure.Storage.Blobs;
using CoffeeAppAPI.Repositories;
using CoffeeAppAPI.Helpers;


namespace CoffeeAppAPI.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadImageAsync(string blobName, string contentType, Stream imageStream);
        Task DeleteImageAsync(Guid id, string ImageUrl);
    }
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IBlobStorageRepository _blobStorageRepository;
        public BlobStorageService(IBlobStorageRepository blobStorageRepository)
        {
            _blobStorageRepository = blobStorageRepository;
        }

        public async Task<string> UploadImageAsync(string blobName, string contentType, Stream imageStream)
        {
            
            return await _blobStorageRepository.UploadImageAsync(blobName, contentType, imageStream);
        }
         public async Task DeleteImageAsync(Guid id, string ImageUrl)
        {
            if (ImageUrl.StartsWith("https://coffeeappstorage.blob.core.windows.net/coffeeappcontainer/"))
            {
                await _blobStorageRepository.DeleteImageAsync(id.ToString());
            }
        }

    }
}