using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface IRepository<T> where T : class, IBaseModel
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(Guid id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }

    // CosmosDbRepository.cs
    public class CosmosDbRepository<T> : IRepository<T> where T : class, IBaseModel
    {
        protected readonly ICosmosDbService _cosmosDbService;
        private readonly Container _container;
        protected Container Container => _container;
        private readonly string _entityType;
        public CosmosDbRepository(ICosmosDbService cosmosDbService, string containerId, string partitionKeyPath, string entityType)
        {
            _cosmosDbService = cosmosDbService;
            _container = _cosmosDbService.GetOrCreateContainerAsync(containerId, partitionKeyPath).GetAwaiter().GetResult();
            _entityType = entityType;
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _cosmosDbService.GetAllItemsAsync<T>(_container, _entityType);
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await _cosmosDbService.GetItemAsync<T>(_container, id.ToString());
        }
    
        public async Task CreateAsync(T entity)
        {
            await _cosmosDbService.AddItemAsync(_container, entity);
        }
        public async Task UpdateAsync(T entity)
        {
            var id = entity.GetType().GetProperty("id").GetValue(entity).ToString();
            await _cosmosDbService.UpdateItemAsync(_container, id, entity);
        }
        public async Task DeleteAsync(Guid id)
        {
            await _cosmosDbService.DeleteItemAsync<T>(_container, id.ToString());
        }
    }
}
