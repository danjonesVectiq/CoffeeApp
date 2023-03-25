using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace CoffeeAppAPI.Services
{

    public interface ICosmosDbService
    {
        Task<Container> GetOrCreateContainerAsync(string containerId, string partitionKeyPath);
        Task<IEnumerable<T>> GetAllItemsAsync<T>(Container container);
        Task<T> GetItemAsync<T>(Container container, string id);
        Task AddItemAsync<T>(Container container, T item);
        Task UpdateItemAsync<T>(Container container, string id, T item);
        Task DeleteItemAsync<T>(Container container, string id);
        Task DeleteAllItemsAsync<T>(Container container) where T : class;
    }

    public class CosmosDbService : ICosmosDbService
    {
        private readonly CosmosClient _cosmosClient;

        public CosmosDbService(IConfiguration configuration)
        {
            var cosmosDbConfig = configuration.GetSection("CosmosDb");
            var connectionString = cosmosDbConfig["ConnectionString"];
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

        public async Task DeleteAllItemsAsync<T>(Container container) where T : class
        {
            var query = container.GetItemQueryIterator<T>(new QueryDefinition("SELECT * FROM c"));
            var itemsToDelete = new List<T>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                itemsToDelete.AddRange(response);
            }

            foreach (var item in itemsToDelete)
            {
                await container.DeleteItemAsync<T>(item.GetType().GetProperty("id").GetValue(item).ToString(), new PartitionKey(item.GetType().GetProperty("id").GetValue(item).ToString()));
            }
        }


    }
}
