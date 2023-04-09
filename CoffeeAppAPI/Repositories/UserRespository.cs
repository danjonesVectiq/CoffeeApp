using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<UserPreferences> LoadUserPreferences(Guid userId);
        Task DeleteAsync(User user);
    }
    public class UserRepository : CosmosDbRepository<User>, IUserRepository
    {
        private readonly IBlobStorageRepository _blobStorageRepository;
        public UserRepository(ICosmosDbService cosmosDbService, IBlobStorageRepository blobStorageRepository)
            : base(cosmosDbService, "User", "/id", "User")
        {
            _blobStorageRepository = blobStorageRepository;
        }
        public async Task<UserPreferences> LoadUserPreferences(Guid userId)
        {
            var user = await GetAsync(userId);
            if (user != null)
            {
                var userPreferences = new UserPreferences
                {
                    CoffeeTypePreferences = user.CoffeeTypePreferences,
                    RoastLevelPreferences = user.RoastLevelPreferences,
                    FlavorNotePreferences = user.FlavorNotePreferences,
                    OriginPreferences = user.OriginPreferences,
                    BrewingMethodPreferences = user.BrewingMethodPreferences
                };
                return userPreferences;
            }
            return null;
        }
         public async Task DeleteAsync(User user)
        {
            if (user != null && !string.IsNullOrEmpty(user.ImageUrl))
            {
                await _blobStorageRepository.DeleteImageAsync(user.id, user.ImageUrl);
            }
            await base.DeleteAsync(user.id);
        }
    }
}
