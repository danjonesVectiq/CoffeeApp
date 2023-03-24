using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace CoffeeApp.Services
{
    public class CosmosDbService
    {
        private readonly CosmosClient _cosmosClient;

        public CosmosDbService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CosmosDb");
            _cosmosClient = new CosmosClient(connectionString);
        }

        public async Task<Container> GetOrCreateContainerAsync(string containerId, string partitionKeyPath)
        {
            var database = _cosmosClient.GetDatabase("CoffeeApp");
            var containerProperties = new ContainerProperties(containerId, partitionKeyPath);
            var containerResponse = await database.CreateContainerIfNotExistsAsync(containerProperties);
            return containerResponse.Container;
        }

        public async Task<IEnumerable<T>> GetAllItemsAsync<T>(Container container)
        {
            var query = new QueryDefinition("SELECT * FROM c");
            var iterator = container.GetItemQueryIterator<T>(query);
            var results = new List<T>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<T> GetItemAsync<T>(Container container, string id)
        {
            try
            {
                ItemResponse<T> response = await container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        public async Task AddItemAsync<T>(Container container, T item)
        {
            await container.CreateItemAsync(item);
        }

        public async Task UpdateItemAsync<T>(Container container, string id, T item)
        {
            await container.ReplaceItemAsync(item, id, new PartitionKey(id));
        }

        public async Task DeleteItemAsync<T>(Container container, string id)
        {
            await container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }

    }
}
