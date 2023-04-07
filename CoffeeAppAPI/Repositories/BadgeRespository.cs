using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface IBadgeRepository : IRepository<Badge>
    {
    }

    public class BadgeRepository : CosmosDbRepository<Badge>, IBadgeRepository
    {
        public BadgeRepository(ICosmosDbService cosmosDbService)
            : base(cosmosDbService, "User", "/id", "Badge")
        {
        }

        public async Task<IEnumerable<Badge>> GetAllBadgesAsync()
        {
            return await GetAllAsync();
        }

        public async Task<Badge> GetBadgeAsync(Guid id)
        {
            return await GetAsync(id);
        }

        public async Task CreateBadgeAsync(Badge badge)
        {
            await CreateAsync(badge);
        }

        public async Task UpdateBadgeAsync(Badge badge)
        {
            await UpdateAsync(badge);
        }

        public async Task DeleteBadgeAsync(Guid id)
        {
            await DeleteAsync(id);
        }
    }
}
