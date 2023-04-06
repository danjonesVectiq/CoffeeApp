using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using CoffeeAppAPI.Models;

namespace CoffeeAppAPI.Services
{

    public interface ICosmosDbService
    {
        Task<Container> GetOrCreateContainerAsync(string containerId, string partitionKeyPath);
        Task<IEnumerable<T>> GetAllItemsAsync<T>(Container container, string itemType) where T : IBaseModel;
        Task<T> GetItemAsync<T>(Container container, string id) where T : IBaseModel;
        Task AddItemAsync<T>(Container container, T item) where T : class, IBaseModel;
        Task UpdateItemAsync<T>(Container container, string id, T item) where T : class, IBaseModel;
        Task DeleteItemAsync<T>(Container container, string id) where T : class, IBaseModel;
        Task DeleteAllItemsAsync<T>(Container container) where T : class, IBaseModel;
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
        public async Task<IEnumerable<T>> GetAllItemsAsync<T>(Container container, string itemType = null) where T : IBaseModel
        {
            IQueryable<T> queryable = container.GetItemLinqQueryable<T>().Where(x => !x.isDeleted);

            if (!string.IsNullOrEmpty(itemType))
            {
                queryable = queryable.Where(x => x.Type == itemType);
            }

            var query = queryable.ToFeedIterator();
            var results = new List<T>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }


        public async Task<T> GetItemAsync<T>(Container container, string id) where T : IBaseModel
        {
            try
            {
                ItemResponse<T> response = await container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource.isDeleted ? default : response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        public async Task AddItemAsync<T>(Container container, T item) where T: class, IBaseModel
        {
            await container.CreateItemAsync(item);
        }

        public async Task UpdateItemAsync<T>(Container container, string id, T item) where T : class, IBaseModel
        {
            await container.ReplaceItemAsync(item, id, new PartitionKey(id));
        }



        public async Task DeleteItemAsync<T>(Container container, string id) where T : class, IBaseModel
        {
            var item = await GetItemAsync<T>(container, id);
            if (item != null)
            {
                item.isDeleted = true;
                await container.ReplaceItemAsync(item, id, new PartitionKey(id));
            }
        }

        public async Task DeleteAllItemsAsync<T>(Container container) where T : class, IBaseModel
        {
            // Changed to async
            var queryable = container.GetItemLinqQueryable<T>().Where(x => !x.isDeleted);
            var query = queryable.ToFeedIterator();
            var items = new List<T>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                items.AddRange(response);
            }

            foreach (var item in items)
            {
                var id = item.id.ToString();
                await container.DeleteItemAsync<T>(id, new PartitionKey(id));
            }
        }

    }
}
