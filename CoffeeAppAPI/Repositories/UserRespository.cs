using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class UserRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public UserRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }
        public async Task<UserPreferences> LoadUserPreferences(Guid userId)
        {
            var usersContainer = await GetUsersContainerAsync();
            var user = await _cosmosDbService.GetItemAsync<CoffeeAppAPI.Models.User>(usersContainer, userId.ToString());

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

        public async Task<Container> GetUsersContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("Users", "/id");
        }

        public async Task<IEnumerable<CoffeeAppAPI.Models.User>> GetAllUsersAsync()
        {
            var usersContainer = await GetUsersContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<CoffeeAppAPI.Models.User>(usersContainer);
        }

        public async Task<CoffeeAppAPI.Models.User> GetUserAsync(string id)
        {
            var usersContainer = await GetUsersContainerAsync();
            return await _cosmosDbService.GetItemAsync<CoffeeAppAPI.Models.User>(usersContainer, id.ToString());
        }

        public async Task CreateUserAsync(CoffeeAppAPI.Models.User user)
        {
            var usersContainer = await GetUsersContainerAsync();
            await _cosmosDbService.AddItemAsync(usersContainer, user);
        }

        public async Task UpdateUserAsync(CoffeeAppAPI.Models.User user)
        {
            var usersContainer = await GetUsersContainerAsync();
            await _cosmosDbService.UpdateItemAsync(usersContainer, user.id.ToString(), user);
        }
    }
}
