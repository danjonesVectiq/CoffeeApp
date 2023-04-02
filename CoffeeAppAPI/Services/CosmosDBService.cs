using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;

namespace CoffeeAppAPI.Services
{

    public interface ICosmosDbService
    {
        Task<Container> GetOrCreateContainerAsync(string containerId, string partitionKeyPath);
        Task<IEnumerable<T>> GetAllItemsAsync<T>(Container container) where T : class;
        Task<T> GetItemAsync<T>(Container container, string id) where T : class;
        Task AddItemAsync<T>(Container container, T item) where T : class;
        Task UpdateItemAsync<T>(Container container, string id, T item) where T : class;
        Task DeleteItemAsync<T>(Container container, string id) where T : class;
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

        public async Task<IEnumerable<T>> GetAllItemsAsync<T>(Container container) where T : class
        {
            var query = container.GetItemLinqQueryable<T>().ToFeedIterator();
            var results = new List<T>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<T> GetItemAsync<T>(Container container, string id) where T : class
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

        public async Task AddItemAsync<T>(Container container, T item) where T : class
        {
            await container.CreateItemAsync(item);
        }

        public async Task UpdateItemAsync<T>(Container container, string id, T item) where T : class
        {
            await container.ReplaceItemAsync(item, id, new PartitionKey(id));
        }

        public async Task DeleteItemAsync<T>(Container container, string id) where T : class
        {
            await container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }

        public async Task DeleteAllItemsAsync<T>(Container container) where T : class
        {
            var query = container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true).AsEnumerable();

            foreach (var item in query)
            {
                var id = item.GetType().GetProperty("id").GetValue(item).ToString();
                await container.DeleteItemAsync<T>(id, new PartitionKey(id));
            }
        }

    }
}
