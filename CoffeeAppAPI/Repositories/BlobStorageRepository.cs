using Azure.Storage.Blobs;
using CoffeeAppAPI.Services;
using CoffeeAppAPI.Helpers;


namespace CoffeeAppAPI.Repositories
{
    public class BlobStorageRepository
    {
        private readonly IBlobStorageService _blobServiceService;
        public BlobStorageRepository(IBlobStorageService blobStorageService)
        {
            _blobServiceService = blobStorageService;
        }

        public async Task<string> UploadImageAsync(string blobName, string contentType, Stream imageStream)
        {
            
            return await _blobServiceService.UploadImageAsync(blobName, contentType, imageStream);
        }

    }
}