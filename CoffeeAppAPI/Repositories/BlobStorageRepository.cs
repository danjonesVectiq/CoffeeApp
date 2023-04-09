using System.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CoffeeAppAPI.Services
{
    public interface IBlobStorageRepository
    {
        Task<string> UploadImageAsync(string blobName, string contentType, Stream imageStream);
        Task DeleteImageAsync(string blobName);
    }
    public class BlobStorageRepository : IBlobStorageRepository
    {
        private readonly BlobServiceClient _blobRepositoryClient;
        private readonly IConfigurationSection _azureStorageConfig;

        public BlobStorageRepository(IConfiguration configuration)
        {
            _azureStorageConfig = configuration.GetSection("AzureStorage");
            var connectionString = _azureStorageConfig["ConnectionString"];
            _blobRepositoryClient = new BlobServiceClient(connectionString);

        }
        public async Task<string> UploadImageAsync(string blobName, string contentType, Stream imageStream)
        {
            var containerClient = _blobRepositoryClient.GetBlobContainerClient(_azureStorageConfig["ContainerName"]);
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

        public async Task DeleteImageAsync(string blobName)
        {
            var containerClient = _blobRepositoryClient.GetBlobContainerClient(_azureStorageConfig["ContainerName"]);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteAsync();
        }
    }


}