using System;
using System.Collections.Generic;

namespace CoffeeAppAPI.Models
{
    public class User
    {
        public Guid id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime JoinDate { get; set; }
        public int TotalCheckins { get; set; }
        public int TotalUniqueCoffees { get; set; }
        public int TotalBadges { get; set; }
        public List<Guid>? FavoriteCoffeeShops { get; set; }
        public List<Guid>? Friends { get; set; }
    }

    public class Coffee
    {
        public Guid id { get; set; }
        public string CoffeeName { get; set; }
        public string CoffeeType { get; set; }
        public string Origin { get; set; }
        public string Roaster { get; set; }
        public string RoastLevel { get; set; }
        public List<string> FlavorNotes { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }

    public class CoffeeShop
    {
        public Guid id { get; set; }
        public string CoffeeShopName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string WebsiteUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string OperatingHours { get; set; }
        public List<Guid> AvailableCoffees { get; set; }
    }

    public class CheckIn
    {
        public Guid id { get; set; }
        public Guid UserId { get; set; }
        public Guid CoffeeId { get; set; }
        public Guid CoffeeShopId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime CheckinDate { get; set; }
        public List<string> CheckinPhotos { get; set; }
    }

    public class Badge
    {
        public Guid Id { get; set; }
        public string BadgeName { get; set; }
        public string BadgeDescription { get; set; }
        public string BadgeIconUrl { get; set; }
        public string BadgeCriteria { get; set; }
    }

    public class UserBadge
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BadgeId { get; set; }
        public DateTime DateEarned { get; set; }
    }

    public class FriendRequest
    {
        public Guid Id { get; set; }
        public Guid RequesterId { get; set; }
        public Guid RecipientId { get; set; }
        public string RequestStatus { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
