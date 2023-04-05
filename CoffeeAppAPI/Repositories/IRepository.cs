using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;


namespace CoffeeAppAPI.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(Guid id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }

    // CosmosDbRepository.cs
    public class CosmosDbRepository<T> : IRepository<T> where T : class
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly Container _container;

        private readonly string _entityType;

        public CosmosDbRepository(ICosmosDbService cosmosDbService, string containerId, string partitionKeyPath, string entityType)
        {
            _cosmosDbService = cosmosDbService;
            _container = _cosmosDbService.GetOrCreateContainerAsync(containerId, partitionKeyPath).GetAwaiter().GetResult();
            _entityType = entityType;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var query = _container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true)
                .Where(item => item.GetType().GetProperty("Type").GetValue(item).ToString() == _entityType)
                .AsEnumerable();

            return query;
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