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
            var coffees = GenerateCoffees(10);
            await SeedCoffees(coffees);

            var coffeeShops = GenerateCoffeeShops(10, coffees);
            await SeedCoffeeShops(coffeeShops);

            List<Guid> coffeeShopIds = coffeeShops.Select(c => c.id).ToList();
            var users = GenerateUsers(coffeeShopIds, 10);
            await SeedUsers(users);




            var checkins = GenerateCheckIns(users, coffees, coffeeShops, 10);
            await SeedCheckins(checkins);

            var badges = GenerateBadges(20);
            await SeedBadges(badges);

            var userBadges = GenerateUserBadges(users, badges, 40);
            await SeedUserBadges(userBadges);

            var friendRequests = GenerateFriendRequests(users, 5);
            await SeedFriendRequests(friendRequests);

            var reviews = GenerateReviews(users, coffees, coffeeShops, 10);
            await SeedReviews(reviews);

            var reviewLikes = GenerateReviewLikes(users, reviews, 10);
            await SeedReviewLikes(reviewLikes);

            var comments = GenerateComments(users, reviews, 10);
            await SeedComments(comments);

            var userFollowings = GenerateUserFollowings(users, 5);
            await SeedUserFollowings(userFollowings);

            var coffeeLists = GenerateCoffeeLists(users, coffees, 10);
            await SeedCoffeeLists(coffeeLists);

            var events = GenerateEvents(coffeeShops, users, 10);
            await SeedEvents(events);

            var userEvents = GenerateUserEvents(users, events, 10);
            await SeedUserEvents(userEvents);

            var notifications = GenerateNotifications(users, 10);
            await SeedNotifications(notifications);





        }

        private async Task SeedUsers(List<CoffeeAppAPI.Models.User> users)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("Users", "/id").Result;
            foreach (var user in users)
            {
                await _cosmosDbService.AddItemAsync(container, user);
            }
        }

        private List<CoffeeAppAPI.Models.User> GenerateUsers(List<Guid> coffeeShopIds, int count)
        {
            var coffeeTypes = new List<string> { "Espresso", "Latte", "Cappuccino", "Americano", "Mocha" };
            var roastLevels = new List<string> { "Light", "Medium", "Medium-Dark", "Dark", "Extra Dark", };
            var flavorNotes = new List<string> { "Chocolate", "Fruity", "Floral", "Nutty", "Spicy", "Sweet" };
            var origins = new List<string> { "Colombia", "Ethiopia", "Brazil", "Guatemala", "Kenya" };
            var brewingMethods = new List<string> { "Pour Over", "French Press", "Aeropress", "Espresso Machine", "Cold Brew" };


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
                .RuleFor(u => u.ProfilePictureUrl, f => f.Internet.Avatar())
                .RuleFor(u => u.TotalCheckins, f => f.Random.Number(1, 100))
                .RuleFor(u => u.TotalUniqueCoffees, f => f.Random.Number(1, 50))
                .RuleFor(u => u.TotalBadges, f => f.Random.Number(1, 20))
            .RuleFor(u => u.FavoriteCoffeeShops, f => f.PickRandom(coffeeShopIds, 3).ToList())
                //.RuleFor(u => u.Friends, f => f.Random.Guid().ToList(5)) Need to  generate a list of friends first implement later
                .RuleFor(u => u.CoffeeTypePreferences, f => f.Make(3, () => new CoffeeTypePreference
                {
                    id = f.Random.Guid(),
                    CoffeeType = f.PickRandom(coffeeTypes),
                    Importance = f.Random.Number(1, 5)
                }))
                .RuleFor(u => u.RoastLevelPreferences, f => f.Make(3, () => new RoastLevelPreference
                {
                    id = f.Random.Guid(),
                    RoastLevel = f.PickRandom(roastLevels),
                    Importance = f.Random.Number(1, 5)
                }))
                .RuleFor(u => u.FlavorNotePreferences, f => f.Make(3, () => new FlavorNotePreference
                {
                    id = f.Random.Guid(),
                    FlavorNote = f.PickRandom(flavorNotes),
                    Importance = f.Random.Number(1, 5)
                }))
                .RuleFor(u => u.OriginPreferences, f => f.Make(3, () => new OriginPreference
                {
                    id = f.Random.Guid(),
                    Origin = f.PickRandom(origins),
                    Importance = f.Random.Number(1, 5)
                }))
                .RuleFor(u => u.BrewingMethodPreferences, f => f.Make(3, () => new BrewingMethodPreference
                {
                    id = f.Random.Guid(),
                    BrewingMethod = f.PickRandom(brewingMethods),
                    Importance = f.Random.Number(1, 5)
                }))
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

        private List<UserBadge> GenerateUserBadges(List<CoffeeAppAPI.Models.User> users, List<Badge> badges, int count)
        {
            var userBadges = new Faker<UserBadge>()
                .RuleFor(ub => ub.id, f => f.Random.Guid())
                .RuleFor(ub => ub.UserId, f => f.PickRandom(users).id)
                .RuleFor(ub => ub.BadgeId, f => f.PickRandom(badges).id)
                .RuleFor(ub => ub.DateEarned, f => f.Date.Past())
                .Generate(count);

            return userBadges;
        }

        private List<FriendRequest> GenerateFriendRequests(List<CoffeeAppAPI.Models.User> users, int count)
        {
            var friendRequests = new Faker<FriendRequest>()
                .RuleFor(fr => fr.id, f => f.Random.Guid())
                .RuleFor(fr => fr.RequesterId, f => f.PickRandom(users).id)
                .RuleFor(fr => fr.RecipientId, f => f.PickRandom(users).id)
                .RuleFor(fr => fr.RequestStatus, f => f.PickRandom(new[] { "Pending", "Accepted", "Declined" }))
                .RuleFor(fr => fr.RequestDate, f => f.Date.Recent())
                .Generate(count);

            return friendRequests;
        }


        private List<Review> GenerateReviews(List<CoffeeAppAPI.Models.User> users, List<Coffee> coffees, List<CoffeeShop> coffeeShops, int count)
        {
            var reviews = new Faker<Review>()
                .RuleFor(r => r.id, f => f.Random.Guid())
                .RuleFor(r => r.UserId, f => f.PickRandom(users).id)
                .RuleFor(r => r.CoffeeId, f => f.PickRandom(coffees).id)
                .RuleFor(r => r.CoffeeShopId, f => f.PickRandom(coffeeShops).id)
                .RuleFor(r => r.Rating, f => f.Random.Number(1, 5))
                .RuleFor(r => r.ReviewText, f => f.Lorem.Paragraph())
                .RuleFor(r => r.ReviewDate, f => f.Date.Past())
                .RuleFor(r => r.ReviewLikesCount, f => f.Random.Number(0, 100))
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

        private List<UserFollowing> GenerateUserFollowings(List<CoffeeAppAPI.Models.User> users, int count)
        {
            var userFollowings = new Faker<UserFollowing>()
                .RuleFor(uf => uf.id, f => f.Random.Guid())
                .RuleFor(uf => uf.FollowerId, f => f.PickRandom(users).id)
                .RuleFor(uf => uf.FolloweeId, f => f.PickRandom(users).id)
                .RuleFor(uf => uf.FollowDate, f => f.Date.Past())
                .Generate(count);

            return userFollowings;
        }

        private List<CoffeeList> GenerateCoffeeLists(List<CoffeeAppAPI.Models.User> users, List<Coffee> coffees, int count)
        {
            var coffeeLists = new Faker<CoffeeList>()
                .RuleFor(cl => cl.id, f => f.Random.Guid())
                .RuleFor(cl => cl.UserId, f => f.PickRandom(users).id)
                .RuleFor(cl => cl.ListName, f => f.Commerce.ProductName())
                .RuleFor(cl => cl.ListDescription, f => f.Lorem.Sentence())
                .RuleFor(cl => cl.CreatedDate, f => f.Date.Past())
                .RuleFor(cl => cl.Coffees, f => f.Random.ListItems(coffees, f.Random.Number(1, coffees.Count)).Select(c => c.id).ToList())
                .Generate(count);

            return coffeeLists;
        }

        private List<Event> GenerateEvents(List<CoffeeShop> coffeeShops, List<CoffeeAppAPI.Models.User> users, int count)
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

        private async Task SeedReviews(List<Review> reviews)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("Reviews", "/id").Result;
            foreach (var review in reviews)
            {
                await _cosmosDbService.AddItemAsync(container, review);
            }
        }

        private async Task SeedReviewLikes(List<ReviewLike> reviewLikes)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("ReviewLikes", "/id").Result;
            foreach (var reviewLike in reviewLikes)
            {
                await _cosmosDbService.AddItemAsync(container, reviewLike);
            }
        }

        private async Task SeedComments(List<Comment> comments)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("Comments", "/id").Result;
            foreach (var comment in comments)
            {
                await _cosmosDbService.AddItemAsync(container, comment);
            }
        }

        private async Task SeedUserFollowings(List<UserFollowing> userFollowings)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("UserFollowings", "/id").Result;
            foreach (var userFollowing in userFollowings)
            {
                await _cosmosDbService.AddItemAsync(container, userFollowing);
            }
        }

        private async Task SeedCoffeeLists(List<CoffeeList> coffeeLists)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("CoffeeLists", "/id").Result;
            foreach (var coffeeList in coffeeLists)
            {
                await _cosmosDbService.AddItemAsync(container, coffeeList);
            }
        }

        private async Task SeedEvents(List<Event> events)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("Events", "/id").Result;
            foreach (var eventItem in events)
            {
                await _cosmosDbService.AddItemAsync(container, eventItem);
            }
        }

        private async Task SeedUserEvents(List<UserEvent> userEvents)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("UserEvents", "/id").Result;
            foreach (var userEvent in userEvents)
            {
                await _cosmosDbService.AddItemAsync(container, userEvent);
            }
        }

        private async Task SeedNotifications(List<Notification> notifications)
        {
            var container = _cosmosDbService.GetOrCreateContainerAsync("Notifications", "/id").Result;
            foreach (var notification in notifications)
            {
                await _cosmosDbService.AddItemAsync(container, notification);
            }
        }


        public async Task CleanUpData()
        {
            // Remove all test data from the development database
            await _cosmosDbService.DeleteAllItemsAsync<FriendRequest>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<CheckIn>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<CoffeeShop>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<CoffeeAppAPI.Models.User>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<Coffee>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<Badge>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<UserBadge>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<Review>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<ReviewLike>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<Comment>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<UserFollowing>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<CoffeeList>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<Event>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<UserEvent>(_container);
            await _cosmosDbService.DeleteAllItemsAsync<Notification>(_container);


        }
    }
}