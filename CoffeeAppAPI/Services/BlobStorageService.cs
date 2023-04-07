using System.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CoffeeAppAPI.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadImageAsync(string blobName, string contentType, Stream imageStream);
    }
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfigurationSection _azureStorageConfig;

        public BlobStorageService(IConfiguration configuration)
        {
            _azureStorageConfig = configuration.GetSection("AzureStorage");
            var connectionString = _azureStorageConfig["ConnectionString"];
            _blobServiceClient = new BlobServiceClient(connectionString);

        }
        public async Task<string> UploadImageAsync(string blobName, string contentType, Stream imageStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_azureStorageConfig["ContainerName"]);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(imageStream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            });

            return blobClient.Uri.AbsoluteUri;
        }
    }


}