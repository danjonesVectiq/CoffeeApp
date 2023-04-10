using Bogus;
using CoffeeAppAPI.Models;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using CoffeeAppAPI.Repositories;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Spatial;
using Azure.Core.GeoJson;
using Microsoft.Azure.Cosmos.Spatial;
using CoffeeAppAPI.Services;

namespace CoffeeAppAPI.Data
{
    public class DataSeeder
    {
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly Container _coffeeContainer;
        private readonly Container _userContainer;
        private readonly Container _interactionContainer;
        private readonly List<string> coffeeTypes = new List<string> { "Espresso", "Latte", "Cappuccino", "Americano", "Mocha" };
        private readonly List<string> roastLevels = new List<string> { "Light", "Medium", "Medium-Dark", "Dark", "Extra Dark", };
        private readonly string[] flavorNotes = new string[] { "Chocolate", "Fruity", "Floral", "Nutty", "Spicy", "Sweet" };
        private readonly List<string> origins = new List<string> { "Colombia", "Ethiopia", "Brazil", "Guatemala", "Kenya" };
        private readonly List<string> grindSizes = new List<string>{
            "Extra Coarse", // Cold Brew, Cowboy Coffee
            "Coarse",       // French Press, Percolator
            "Medium-Coarse",// Chemex, Clever Dripper
            "Medium",       // Drip Coffee Makers, Siphon, Aeropress (3+ minutes brew time)
            "Medium-Fine",  // Pour Over, V60, Aeropress (1-2 minutes brew time)
            "Fine",         // Espresso, Moka Pot, Aeropress (30 seconds brew time)
            "Extra Fine",   // Turkish Coffee
        };
        private readonly List<string> brewingMethods = new List<string> { "Pour Over", "French Press", "Aeropress", "Espresso Machine", "Cold Brew" };

        private readonly IRecommendationService _recommendationService;
        private readonly ICoffeeScoringService _coffeeScoringService;

        private readonly IServiceProvider _serviceProvider;
        public DataSeeder(ICosmosDbRepository cosmosDbRepository, ICoffeeScoringService coffeeSimilarityMatrix, IServiceProvider serviceProvider)
        {
            _cosmosDbRepository = cosmosDbRepository;
            _userContainer = cosmosDbRepository.GetOrCreateContainerAsync("User", "/id").Result;
            _coffeeContainer = cosmosDbRepository.GetOrCreateContainerAsync("Coffee", "/id").Result;
            _interactionContainer = cosmosDbRepository.GetOrCreateContainerAsync("Interaction", "/id").Result;
            //_recommendationService = recommendationService;
            _coffeeScoringService = coffeeSimilarityMatrix;
            _serviceProvider = serviceProvider;
        }
        public async Task SeedData()
        {
            var badges = GenerateBadges(20);
            await SeedBadges(badges);
            var roasters = GenerateRoasters(10);
            await SeedRoasters(roasters);
            var coffees = GenerateCoffees(roasters, 4);
            await SeedCoffees(coffees);
            var coffeeShops = GenerateCoffeeShops(10, coffees);
            await SeedCoffeeShops(coffeeShops);

            List<Guid> coffeeShopIds = coffeeShops.Select(c => c.id).ToList();
            var users = GenerateUsers(coffeeShopIds, badges, 10);

            //update users with recommendations
            users = GenerateRecommendations(users, coffees, 3);
            await SeedUsers(users);
            var checkins = GenerateCheckIns(users, coffees, coffeeShops, 10);
            await SeedCheckins(checkins);
            var recipes = GenerateRecipes(coffees, users, 10);
            await SeedRecipes(recipes);


            /*             var reviews = GenerateReviews(users, coffees, coffeeShops, 10);
            await SeedReviews(reviews);
            var comments = GenerateComments(users, reviews, 10);
            await SeedComments(comments);
            var notifications = GenerateNotifications(users, 10);
            await SeedNotifications(notifications); */

            /* var friendRequests = GenerateFriendRequests(users, 5);
           await SeedFriendRequests(friendRequests); */

            /*   var userFollowings = GenerateUserFollowings(users, 5);
              await SeedUserFollowings(userFollowings);*/

            /* var events = GenerateEvents(coffeeShops, users, 10);
            await SeedEvents(events); */

            /* var userEvents = GenerateUserEvents(users, events, 10);
            await SeedUserEvents(userEvents); */
        }
        private List<CoffeeAppAPI.Models.User> GenerateUsers(List<Guid> coffeeShopIds, List<Badge> badges, int count)
        {
            // Generate a list of fake users using Bogus
            return new Faker<CoffeeAppAPI.Models.User>()
                .RuleFor(u => u.id, f => f.Random.Guid())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Username, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.Password, f => f.Internet.Password())
                .RuleFor(u => u.JoinDate, f => f.Date.Past())
                .RuleFor(u => u.Bio, f => f.Lorem.Sentence())
                .RuleFor(u => u.ImageUrl, f => f.Internet.Avatar())
                .RuleFor(u => u.TotalCheckins, f => f.Random.Number(1, 100))
                .RuleFor(u => u.TotalUniqueCoffees, f => f.Random.Number(1, 50))
                .RuleFor(u => u.TotalBadges, f => f.Random.Number(1, 20))
                .RuleFor(u => u.FavoriteCoffeeShops, f => f.PickRandom(coffeeShopIds, 3).ToList())
                .RuleFor(u => u.Badges, f => f.PickRandom(badges, 3).ToList())
                //.RuleFor(u => u.Friends, f => f.Random.Guid().ToList(5)) Need to generate a list of friends first implement later
                .RuleFor(u => u.Preferences, f => new UserPreferences
                {
                    CoffeeTypePreferences = f.PickRandom(coffeeTypes, 3).ToList(),
                    RoastLevelPreferences = f.PickRandom(roastLevels, 3).ToList(),
                    FlavorNotePreferences = f.PickRandom(flavorNotes, 3).ToList(),
                    OriginPreferences = f.PickRandom(origins, 3).ToList(),
                    BrewingMethodPreferences = f.PickRandom(brewingMethods, 3).ToList()
                })
                .Generate(count);
        }
        private List<Recipe> GenerateRecipes(List<Coffee> coffee, List<Models.User> users, int count)
        {
            return new Faker<Recipe>()
                .RuleFor(r => r.id, f => f.Random.Guid())
                .RuleFor(r => r.CoffeeId, f => f.PickRandom(coffee).id)
                .RuleFor(r => r.UserId, f => f.PickRandom(users).id)
                .RuleFor(r => r.BrewingMethod, f => f.PickRandom(brewingMethods))
                .RuleFor(r => r.BrewTime, f => f.Random.Number(1, 10))
                .RuleFor(r => r.Temperature, f => f.Random.Number(1, 10))
                .RuleFor(r => r.GrindSize, f => f.PickRandom(grindSizes))
                .RuleFor(r => r.WaterAmount, f => f.Random.Number(1, 10))
                .RuleFor(r => r.CoffeeAmount, f => f.Random.Number(1, 10))
                .RuleFor(r => r.Instructions, f => f.Lorem.Sentence())
                .RuleFor(r => r.PersonalExperience, f => f.Lorem.Sentence())
                .RuleFor(r => r.RecipeName, f => f.Lorem.Sentence())
                // .RuleFor(r => r.Rating, f => f.Random.Number(1, 5))
                // .RuleFor(r => r.Review, f => f.Lorem.Sentence())
                .RuleFor(r => r.AddedDate, f => f.Date.Past())
                .Generate(count);
        }
        private List<Roaster> GenerateRoasters(int count)
        {
            var roasters = new Faker<Roaster>()
                .RuleFor(r => r.id, f => f.Random.Guid())
                .RuleFor(r => r.RoasterName, f => f.Company.CompanyName())
                .RuleFor(r => r.ImageUrl, f => f.Internet.Avatar())
                .RuleFor(r => r.Address, f => f.Address.StreetAddress())
                .RuleFor(r => r.City, f => f.Address.City())
                .RuleFor(r => r.State, f => f.Address.State())
                .RuleFor(r => r.Country, f => f.Address.Country())
                .RuleFor(r => r.Latitude, f => f.Address.Latitude())
                .RuleFor(r => r.Longitude, f => f.Address.Longitude())
                .RuleFor(r => r.WebsiteUrl, f => f.Internet.Url())
                .RuleFor(r => r.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(r => r.Description, f => f.Lorem.Paragraph())
                .Generate(count);
            return roasters;
        }
        private List<Coffee> GenerateCoffees(List<Roaster> roasters, int count)
        {
            var coffees = new Faker<Coffee>()
                .RuleFor(c => c.id, f => f.Random.Guid())
                .RuleFor(c => c.CoffeeName, f => f.Commerce.ProductName())
                .RuleFor(c => c.ImageUrl, f => f.Internet.Avatar())
                .RuleFor(c => c.CoffeeType, f => f.PickRandom(this.coffeeTypes))
                .RuleFor(c => c.Origin, f => f.Address.Country())
                .RuleFor(c => c.Roaster, f => f.PickRandom(roasters))
                .RuleFor(c => c.RoastLevel, f => f.PickRandom(this.roastLevels))
                .RuleFor(c => c.FlavorNotes, f => f.Random.Shuffle(this.flavorNotes).Take(f.Random.Int(1, 3)).ToList())
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
                .RuleFor(cs => cs.ImageUrl, f => f.Internet.Avatar())
                .RuleFor(cs => cs.Address, f => f.Address.StreetAddress())
                .RuleFor(cs => cs.City, f => f.Address.City())
                .RuleFor(cs => cs.State, f => f.Address.State())
                .RuleFor(cs => cs.Country, f => f.Address.Country())
                .RuleFor(c => c.Location, f => new Point(
                    f.Random.Double(-35.458000, -35.126500), // Latitude range for Canberra
                    f.Random.Double(148.968100, 149.281300) // Longitude range for Canberra
                ))
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
                .RuleFor(ch => ch.User, f => f.PickRandom(users))
                .RuleFor(ch => ch.Coffees, f => f.PickRandom(coffees, f.Random.Number(1, 3)).ToList())
                .RuleFor(ch => ch.CoffeeShop, f => f.PickRandom(coffeeShops))
                .RuleFor(ch => ch.CheckinDate, f => f.Date.Past())
                .RuleFor(ch => ch.CheckinPhotos, f => new List<string>()) // You can populate this with image URLs
                .Generate(count);
            return checkins;
        }
        private List<Badge> GenerateBadges(int count)
        {
            var badgeList = new List<(string BadgeName, string BadgeDescription)>
            {                
                #region Badgenames
                ("Espresso Explorer", "Check-in at 5 different coffee shops."),
                ("Bean Connoisseur", "Try 10 different types of coffee beans."),
                ("Latte Art Lover", "Share 5 photos of latte art on the app."),
                ("Cold Brew Crew", "Log 10 cold brew coffee check-ins."),
                ("Caffeine Crawl", "Visit 3 coffee shops in a single day."),
                ("Home Barista", "Log 5 check-ins of home-brewed coffee."),
                ("International Coffee Hopper", "Check-in at coffee shops in 3 different countries."),
                ("Early Riser", "Check-in before 7:00 AM, 10 times."),
                ("Night Owl", "Check-in after 9:00 PM, 10 times."),
                ("Coffee and Conversation", "Check-in with a friend at a coffee shop 5 times."),
                ("Study Buddy", "Check-in at a coffee shop while working or studying, 10 times."),
                ("Seasonal Sipper", "Try 5 seasonal or limited-time coffee beverages."),
                ("Decaf Dabbler", "Log 5 decaf coffee check-ins."),
                ("Sweet Tooth", "Try 5 different flavored coffee beverages (e.g., mocha, caramel, vanilla)."),
                ("Eco-Friendly Cup", "Check-in 10 times with a reusable cup."),
                ("Local Coffee Lover", "Check-in at 5 independent, locally-owned coffee shops."),
                ("Coffee Festival Fanatic", "Attend a coffee festival or event and check-in."),
                ("Coffee Passport", "Try coffee from 5 different coffee-producing countries."),
                ("Plant-Based Barista", "Log 10 check-ins with plant-based milk alternatives (e.g., almond, soy, oat)."),
                ("Milestone Mocha", "Celebrate a personal or professional milestone with a coffee check-in."),
                ("Drip Drop Champion", "Try 5 different drip coffee methods."),
                ("Coffee Pairing Pro", "Check-in 5 times with a food pairing."),
                ("Affogato Aficionado", "Try 3 different affogato creations."),
                ("Aeropress Ace", "Log 5 check-ins with Aeropress-brewed coffee."),
                ("French Press Fan", "Log 5 check-ins with French press-brewed coffee."),
                ("Nitro Boost", "Try 3 different nitro cold brew coffee drinks."),
                ("Sustainable Sipper", "Check-in 5 times at eco-friendly coffee shops."),
                ("Roastery Rambler", "Visit 3 different coffee roasteries."),
                ("Espresso Enthusiast", "Try 5 different espresso-based drinks."),
                ("Chemex Chemist", "Log 5 check-ins with Chemex-brewed coffee.")
                #endregion
            };
            var badges = new Faker<Badge>()
                .RuleFor(b => b.id, f => f.Random.Guid())
                .CustomInstantiator(f =>
                {
                    var randomBadge = f.Random.ListItem(badgeList);
                    return new Badge
                    {
                        BadgeName = randomBadge.BadgeName,
                        BadgeDescription = randomBadge.BadgeDescription
                    };
                })
                .RuleFor(b => b.BadgeIconUrl, f => f.Internet.Avatar())
                .RuleFor(b => b.BadgeCriteria, f => f.Lorem.Sentence())
                .Generate(count);
            return badges;
        }
        /*   private List<Review> GenerateReviews(List<CoffeeAppAPI.Models.User> users, List<Coffee> coffees, List<CoffeeShop> coffeeShops, int count)
          {
              var reviews = new Faker<Review>()
                  .RuleFor(r => r.id, f => f.Random.Guid())
                  .RuleFor(r => r.UserId, f => f.PickRandom(users).id)
                  .RuleFor(r => r.CoffeeId, f => f.PickRandom(coffees).id)
                  .RuleFor(r => r.CoffeeShopId, f => f.Random.Bool() ? f.PickRandom(coffeeShops).id : (Guid?)null)
                  .RuleFor(r => r.Rating, f => f.Random.Number(1, 5))
                  .RuleFor(r => r.NormalizedRating, f => Math.Round(f.Random.Double(1, 5), 1))
                  .RuleFor(r => r.ReviewText, f => f.Lorem.Paragraph())
                  .RuleFor(r => r.ReviewDate, f => f.Date.Past(3))
                  .Generate(count);
              return reviews;
          }
          private List<ReviewLike> GenerateReviewLikes(List<CoffeeAppAPI.Models.User> users, List<Review> reviews, int count)
          {
              var reviewLikes = new Faker<ReviewLike>()
                  .RuleFor(rl => rl.id, f => f.Random.Guid())
                  .RuleFor(rl => rl.UserId, f => f.PickRandom(users).id)
                  .RuleFor(rl => rl.ReviewId, f => f.PickRandom(reviews).id)
                  .RuleFor(rl => rl.LikedDate, f => f.Date.Recent())
                  .Generate(count);
              return reviewLikes;
          }
          private List<Comment> GenerateComments(List<CoffeeAppAPI.Models.User> users, List<Review> reviews, int count)
          {
              var comments = new Faker<Comment>()
                  .RuleFor(c => c.id, f => f.Random.Guid())
                  .RuleFor(c => c.UserId, f => f.PickRandom(users).id)
                  .RuleFor(c => c.ReviewId, f => f.PickRandom(reviews).id)
                  .RuleFor(c => c.CommentText, f => f.Lorem.Sentence())
                  .RuleFor(c => c.CommentDate, f => f.Date.Recent())
                  .Generate(count);
              return comments;
          }
          private List<Notification> GenerateNotifications(List<CoffeeAppAPI.Models.User> users, int count)
          {
              var notifications = new Faker<Notification>()
                  .RuleFor(n => n.id, f => f.Random.Guid())
                  .RuleFor(n => n.UserId, f => f.PickRandom(users).id)
                  .RuleFor(n => n.NotificationType, f => f.PickRandom(new[] { "Like", "Comment", "Event Invitation", "Friend Request" }))
                  .RuleFor(n => n.Content, f => f.Lorem.Sentence())
                  .RuleFor(n => n.CreatedDate, f => f.Date.Recent())
                  .RuleFor(n => n.IsRead, f => f.Random.Bool())
                  .Generate(count);

              return notifications;
          } */
        /*  private List<Event> GenerateEvents(List<CoffeeShop> coffeeShops, List<CoffeeAppAPI.Models.User> users, int count)
        {
           var events = new Faker<Event>()
               .RuleFor(e => e.id, f => f.Random.Guid())
               .RuleFor(e => e.EventName, f => f.Company.CatchPhrase())
               .RuleFor(e => e.EventDescription, f => f.Lorem.Paragraph())
               .RuleFor(e => e.EventDate, f => f.Date.Future())
               .RuleFor(e => e.EventLocation, f => f.Address.FullAddress())
               .RuleFor(e => e.CoffeeShopId, f => f.PickRandom(coffeeShops).id)
               .RuleFor(e => e.OrganizerId, f => f.PickRandom(users).id)
               .Generate(count);

           return events;
        }

        private List<UserEvent> GenerateUserEvents(List<CoffeeAppAPI.Models.User> users, List<Event> events, int count)
        {
           var userEvents = new Faker<UserEvent>()
               .RuleFor(ue => ue.id, f => f.Random.Guid())
               .RuleFor(ue => ue.UserId, f => f.PickRandom(users).id)
               .RuleFor(ue => ue.EventId, f => f.PickRandom(events).id)
               .RuleFor(ue => ue.Status, f => f.PickRandom(new[] { "Going", "Interested", "Not Going" }))
               .Generate(count);

           return userEvents;
        } */


        /*  private List<UserFollowing> GenerateUserFollowings(List<CoffeeAppAPI.Models.User> users, int count)
         {
             var userFollowings = new Faker<UserFollowing>()
                 .RuleFor(uf => uf.id, f => f.Random.Guid())
                 .RuleFor(uf => uf.FollowerId, f => f.PickRandom(users).id)
                 .RuleFor(uf => uf.FolloweeId, f => f.PickRandom(users).id)
                 .RuleFor(uf => uf.FollowDate, f => f.Date.Past())
                 .Generate(count);

             return userFollowings;
         } */

        /*   private List<FriendRequest> GenerateFriendRequests(List<CoffeeAppAPI.Models.User> users, int count)
        {
           var friendRequests = new Faker<FriendRequest>()
               .RuleFor(fr => fr.id, f => f.Random.Guid())
               .RuleFor(fr => fr.RequesterId, f => f.PickRandom(users).id)
               .RuleFor(fr => fr.RecipientId, f => f.PickRandom(users).id)
               .RuleFor(fr => fr.RequestStatus, f => f.PickRandom(new[] { "Pending", "Accepted", "Declined" }))
               .RuleFor(fr => fr.RequestDate, f => f.Date.Recent())
               .Generate(count);

           return friendRequests;
        } */

        public List<Models.User> GenerateRecommendations(List<Models.User> allUsers, List<Coffee> allCoffees, int numberOfRecommendations)
        {
            var container = _cosmosDbRepository.GetOrCreateContainerAsync("Coffee", "/id").Result;
            var cs = _cosmosDbRepository.GetAllItemsAsync<Coffee>(container, "Coffee").Result.ToList();
            _coffeeScoringService.UpdateCoffeeSimilarityMatrix(cs);
            var Matrix = _coffeeScoringService.Matrix;
            foreach (var user in allUsers)
            {
                var recommendations = new List<Recommendation>(); 
                foreach (Guid coffeeId in _coffeeScoringService.GenerateRecommendations(user, allCoffees, 3))
                {
                    recommendations.Add(new Recommendation
                    {
                        id = Guid.NewGuid(),
                        UserId = user.id,
                        CoffeeId = coffeeId,
                        GeneratedOn = DateTime.Now
                    });
                }
                user.Recommendations = recommendations;
            }
            return allUsers;
        }

        private async Task SeedUsers(List<CoffeeAppAPI.Models.User> users)
        {
            var container = _cosmosDbRepository.GetOrCreateContainerAsync("User", "/id").Result;
            foreach (var user in users)
            {
                await _cosmosDbRepository.AddItemAsync(container, user);
            }
        }
        private async Task SeedBadges(List<Badge> badges)
        {
            var container = _cosmosDbRepository.GetOrCreateContainerAsync("User", "/id").Result;
            foreach (var badge in badges)
            {
                await _cosmosDbRepository.AddItemAsync(container, badge);
            }
        }

        private async Task SeedNotifications(List<Notification> notifications)
        {
            var container = _cosmosDbRepository.GetOrCreateContainerAsync("User", "/id").Result;
            foreach (var notification in notifications)
            {
                await _cosmosDbRepository.AddItemAsync(container, notification);
            }
        }
        private async Task SeedRoasters(List<Roaster> roasters)
        {
            var container = _cosmosDbRepository.GetOrCreateContainerAsync("Coffee", "/id").Result;
            foreach (var roaster in roasters)
            {
                await _cosmosDbRepository.AddItemAsync(container, roaster);
            }
        }
        private async Task SeedCoffees(List<Coffee> coffees)
        {
            var container = _cosmosDbRepository.GetOrCreateContainerAsync("Coffee", "/id").Result;
            foreach (var coffee in coffees)
            {
                await _cosmosDbRepository.AddItemAsync(container, coffee);
            }
        }

        private async Task SeedCoffeeShops(List<CoffeeShop> coffeeShops)
        {
            var container = _cosmosDbRepository.GetOrCreateContainerAsync("Coffee", "/id").Result;
            foreach (var coffeeShop in coffeeShops)
            {
                await _cosmosDbRepository.AddItemAsync(container, coffeeShop);
            }
        }

        private async Task SeedRecipes(List<Recipe> recipes)
        {
            var container = _cosmosDbRepository.GetOrCreateContainerAsync("Coffee", "/id").Result;
            foreach (var recipe in recipes)
            {
                await _cosmosDbRepository.AddItemAsync(container, recipe);
            }
        }

        private async Task SeedCheckins(List<CheckIn> checkins)
        {
            var container = _cosmosDbRepository.GetOrCreateContainerAsync("Interaction", "/id").Result;
            foreach (var checkin in checkins)
            {
                await _cosmosDbRepository.AddItemAsync(container, checkin);
            }
        }

        private async Task SeedReviews(List<Review> reviews)
        {
            var container = _cosmosDbRepository.GetOrCreateContainerAsync("Interaction", "/id").Result;
            foreach (var review in reviews)
            {
                await _cosmosDbRepository.AddItemAsync(container, review);
            }
        }

        private async Task SeedComments(List<Comment> comments)
        {
            var container = _cosmosDbRepository.GetOrCreateContainerAsync("Interaction", "/id").Result;
            foreach (var comment in comments)
            {
                await _cosmosDbRepository.AddItemAsync(container, comment);
            }
        }

        /* 
               private async Task SeedFriendRequests(List<FriendRequest> friendRequests)
               {
                   var container = _cosmosDbRepository.GetOrCreateContainerAsync("FriendRequests", "/id").Result;
                   foreach (var friendRequest in friendRequests)
                   {
                       await _cosmosDbRepository.AddItemAsync(container, friendRequest);
                   }
               } */


        /*  private async Task SeedUserFollowings(List<UserFollowing> userFollowings)
         {
             var container = _cosmosDbRepository.GetOrCreateContainerAsync("UserFollowings", "/id").Result;
             foreach (var userFollowing in userFollowings)
             {
                 await _cosmosDbRepository.AddItemAsync(container, userFollowing);
             }
         } */
        /*      private async Task SeedEvents(List<Event> events)
             {
                 var container = _cosmosDbRepository.GetOrCreateContainerAsync("Events", "/id").Result;
                 foreach (var eventItem in events)
                 {
                     await _cosmosDbRepository.AddItemAsync(container, eventItem);
                 }
             } */

        /*         private async Task SeedUserEvents(List<UserEvent> userEvents)
                {
                    var container = _cosmosDbRepository.GetOrCreateContainerAsync("UserEvents", "/id").Result;
                    foreach (var userEvent in userEvents)
                    {
                        await _cosmosDbRepository.AddItemAsync(container, userEvent);
                    }
                } */




        public async Task CleanUpData()
        {
            // Remove all test data from the development database
            // await _cosmosDbRepository.DeleteAllItemsAsync<FriendRequest>(_container);
            await _cosmosDbRepository.DeleteAllItemsAsync<Roaster>(_coffeeContainer);
            await _cosmosDbRepository.DeleteAllItemsAsync<CheckIn>(_interactionContainer);
            await _cosmosDbRepository.DeleteAllItemsAsync<CoffeeShop>(_coffeeContainer);
            await _cosmosDbRepository.DeleteAllItemsAsync<Models.User>(_userContainer);
            await _cosmosDbRepository.DeleteAllItemsAsync<Coffee>(_coffeeContainer);
            await _cosmosDbRepository.DeleteAllItemsAsync<Badge>(_userContainer);
            await _cosmosDbRepository.DeleteAllItemsAsync<Review>(_interactionContainer);
            await _cosmosDbRepository.DeleteAllItemsAsync<Recipe>(_coffeeContainer);
            await _cosmosDbRepository.DeleteAllItemsAsync<Recommendation>(_coffeeContainer);
            //await _cosmosDbRepository.DeleteAllItemsAsync<ReviewLike>(_container);
            await _cosmosDbRepository.DeleteAllItemsAsync<Comment>(_interactionContainer);
            // await _cosmosDbRepository.DeleteAllItemsAsync<UserFollowing>(_container);
            await _cosmosDbRepository.DeleteAllItemsAsync<Notification>(_userContainer);
            /* 
                        await _cosmosDbRepository.DeleteAllItemsAsync<Event>(_container);
                        await _cosmosDbRepository.DeleteAllItemsAsync<UserEvent>(_container); */



        }
    }
}