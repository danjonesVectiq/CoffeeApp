using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public interface IService<T> where T : class, IBaseModel
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(Guid id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }

    // CosmosDbService.cs
    public class CosmosDbService<T> : IService<T> where T : class, IBaseModel
    {
        protected readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly Container _container;
        protected Container Container => _container;
        private readonly string _entityType;
        public CosmosDbService(ICosmosDbRepository cosmosDbRepository, string containerId, string partitionKeyPath, string entityType)
        {
            _cosmosDbRepository = cosmosDbRepository;
            _container = _cosmosDbRepository.GetOrCreateContainerAsync(containerId, partitionKeyPath).GetAwaiter().GetResult();
            _entityType = entityType;
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _cosmosDbRepository.GetAllItemsAsync<T>(_container, _entityType);
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await _cosmosDbRepository.GetItemAsync<T>(_container, id.ToString());
        }
    
        public async Task CreateAsync(T entity)
        {
            await _cosmosDbRepository.AddItemAsync(_container, entity);
        }
        public async Task UpdateAsync(T entity)
        {
            var id = entity.GetType().GetProperty("id").GetValue(entity).ToString();
            await _cosmosDbRepository.UpdateItemAsync(_container, id, entity);
        }
        public async Task DeleteAsync(Guid id)
        {
            await _cosmosDbRepository.DeleteItemAsync<T>(_container, id.ToString());
        }
    }
}
