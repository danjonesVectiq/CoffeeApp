using Bogus;
using CoffeeAppAPI.Models;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using CoffeeAppAPI.Services;

using Microsoft.Extensions.DependencyInjection;

namespace CoffeeAppAPI.Data
{
    public class DataSeeder
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly Container _container;

        public DataSeeder(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
            _container = cosmosDbService.GetOrCreateContainerAsync("Users", "/id").Result;
        }
        public async Task SeedData()
        {
            var users = GenerateUsers(10);
            await SeedUsers(users);

            var coffees = GenerateCoffees(10);
            await SeedCoffees(coffees);

            var coffeeShops = GenerateCoffeeShops(10, coffees);
            await SeedCoffeeShops(coffeeShops);

            var checkins = GenerateCheckIns(users, coffees, coffeeShops, 10);
            await SeedCheckins(checkins);

            var badges = GenerateBadges(5);
            await SeedBadges(badges);

            var userBadges = GenerateUserBadges(users, badges, 10);
            await SeedUserBadges(userBadges);

            var friendRequests = GenerateFriendRequests(users, 10);
            await SeedFriendRequests(friendRequests);
        }

        private async Task SeedUsers(List<CoffeeAppAPI.Models.User> users)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("Users", "/id").Result;
            foreach (var user in users)
            {
                await _cosmosDbService.AddItemAsync(container, user);
            }
        }

        private List<CoffeeAppAPI.Models.User> GenerateUsers(int count)
        {
            // Generate a list of fake users using Bogus
            return new Faker<CoffeeAppAPI.Models.User>()
            .RuleFor(u => u.id, f => f.Random.Guid())
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(u => u.Password, f => f.Internet.Password())
            .RuleFor(u => u.JoinDate, f => f.Date.Past())

                .Generate(count);
        }



        private List<Coffee> GenerateCoffees(int count)
        {
            var coffees = new Faker<Coffee>()
                .RuleFor(c => c.id, f => f.Random.Guid())
                .RuleFor(c => c.CoffeeName, f => f.Commerce.ProductName())
                .RuleFor(c => c.CoffeeType, f => f.PickRandom(new[] { "Espresso", "Cold Brew", "Pour Over", "Drip", "French Press" }))
                .RuleFor(c => c.Origin, f => f.Address.Country())
                .RuleFor(c => c.Roaster, f => f.Company.CompanyName())
                .RuleFor(c => c.RoastLevel, f => f.PickRandom(new[] { "Light", "Medium", "Dark" }))
                .RuleFor(c => c.FlavorNotes, f => f.Random.ListItems(new[] { "Chocolate", "Caramel", "Fruity", "Floral", "Nutty" }, f.Random.Number(1, 3)))
                .RuleFor(c => c.AverageRating, f => Math.Round(f.Random.Double(1, 5), 1))
                .RuleFor(c => c.TotalRatings, f => f.Random.Number(1, 1000))
                .Generate(count);

            return coffees;
        }

        private List<CoffeeShop> GenerateCoffeeShops(int count, List<Coffee> coffees)
        {
            var coffeeShops = new Faker<CoffeeShop>()
                .RuleFor(cs => cs.id, f => f.Random.Guid())
                .RuleFor(cs => cs.CoffeeShopName, f => f.Company.CompanyName())
                .RuleFor(cs => cs.Address, f => f.Address.StreetAddress())
                .RuleFor(cs => cs.City, f => f.Address.City())
                .RuleFor(cs => cs.State, f => f.Address.State())
                .RuleFor(cs => cs.Country, f => f.Address.Country())
                .RuleFor(cs => cs.Latitude, f => f.Address.Latitude())
                .RuleFor(cs => cs.Longitude, f => f.Address.Longitude())
                .RuleFor(cs => cs.WebsiteUrl, f => f.Internet.Url())
                .RuleFor(cs => cs.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(cs => cs.OperatingHours, f => "8:00 AM - 8:00 PM")
                .RuleFor(cs => cs.AvailableCoffees, f => f.Random.ListItems(coffees, f.Random.Number(1, coffees.Count)).Select(c => c.id).ToList())
                .Generate(count);

            return coffeeShops;
        }



        private List<CheckIn> GenerateCheckIns(List<CoffeeAppAPI.Models.User> users, List<Coffee> coffees, List<CoffeeShop> coffeeShops, int count)
        {
            var checkins = new Faker<CheckIn>()
                .RuleFor(ch => ch.id, f => f.Random.Guid())
                .RuleFor(ch => ch.UserId, f => f.PickRandom(users).id)
                .RuleFor(ch => ch.CoffeeId, f => f.PickRandom(coffees).id)
                .RuleFor(ch => ch.CoffeeShopId, f => f.PickRandom(coffeeShops).id)
                .RuleFor(ch => ch.Rating, f => f.Random.Number(1, 5))
                .RuleFor(ch => ch.ReviewText, f => f.Lorem.Paragraph())
                .RuleFor(ch => ch.CheckinDate, f => f.Date.Past())
                .RuleFor(ch => ch.CheckinPhotos, f => new List<string>()) // You can populate this with image URLs
                .Generate(count);

            return checkins;
        }



        private List<Badge> GenerateBadges(int count)
        {
            var badges = new Faker<Badge>()
                .RuleFor(b => b.Id, f => f.Random.Guid())
                .RuleFor(b => b.BadgeName, f => f.Commerce.ProductName())
                .RuleFor(b => b.BadgeDescription, f => f.Lorem.Sentence())
                .RuleFor(b => b.BadgeIconUrl, f => f.Internet.Avatar())
                .RuleFor(b => b.BadgeCriteria, f => f.Lorem.Sentence())
                .Generate(count);

            return badges;
        }

        private List<UserBadge> GenerateUserBadges(List<CoffeeAppAPI.Models.User> users, List<Badge> badges, int count)
        {
            var userBadges = new Faker<UserBadge>()
                .RuleFor(ub => ub.Id, f => f.Random.Guid())
                .RuleFor(ub => ub.UserId, f => f.PickRandom(users).id)
                .RuleFor(ub => ub.BadgeId, f => f.PickRandom(badges).Id)
                .RuleFor(ub => ub.DateEarned, f => f.Date.Past())
                .Generate(count);

            return userBadges;
        }

        private List<FriendRequest> GenerateFriendRequests(List<CoffeeAppAPI.Models.User> users, int count)
        {
            var friendRequests = new Faker<FriendRequest>()
                .RuleFor(fr => fr.Id, f => f.Random.Guid())
                .RuleFor(fr => fr.RequesterId, f => f.PickRandom(users).id)
                .RuleFor(fr => fr.RecipientId, f => f.PickRandom(users).id)
                .RuleFor(fr => fr.RequestStatus, f => f.PickRandom(new[] { "Pending", "Accepted", "Declined" }))
                .RuleFor(fr => fr.RequestDate, f => f.Date.Recent())
                .Generate(count);

            return friendRequests;
        }

        private async Task SeedCoffees(List<Coffee> coffees)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("Coffees", "/id").Result;
            foreach (var coffee in coffees)
            {
                await _cosmosDbService.AddItemAsync(container, coffee);
            }
        }

        private async Task SeedCoffeeShops(List<CoffeeShop> coffeeShops)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("CoffeeShops", "/id").Result;
            foreach (var coffeeShop in coffeeShops)
            {
                await _cosmosDbService.AddItemAsync(container, coffeeShop);
            }
        }

        private async Task SeedCheckins(List<CheckIn> checkins)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("CheckIns", "/id").Result;
            foreach (var checkin in checkins)
            {
                await _cosmosDbService.AddItemAsync(container, checkin);
            }
        }

        private async Task SeedBadges(List<Badge> badges)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("Badges", "/id").Result;
            foreach (var badge in badges)
            {
                await _cosmosDbService.AddItemAsync(container, badge);
            }
        }

        private async Task SeedUserBadges(List<UserBadge> userBadges)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("UserBadges", "/id").Result;
            foreach (var userBadge in userBadges)
            {
                await _cosmosDbService.AddItemAsync(container, userBadge);
            }
        }

        private async Task SeedFriendRequests(List<FriendRequest> friendRequests)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("FriendRequests", "/id").Result;
            foreach (var friendRequest in friendRequests)
            {
                await _cosmosDbService.AddItemAsync(container, friendRequest);
            }
        }

        public async Task CleanUpData()
        {
            // Remove all test data from the development database
            await _cosmosDbService.DeleteAllItemsAsync<CoffeeAppAPI.Models.User>(_container);
        }
    }
}