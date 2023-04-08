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
    }
    public class UserRepository : CosmosDbRepository<User>, IUserRepository
    {
        public UserRepository(ICosmosDbService cosmosDbService)
            : base(cosmosDbService, "User", "/id", "User")
        {
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
    }
}
