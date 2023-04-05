using System;
using System.Collections.Generic;

namespace CoffeeAppAPI.Models
{
    public interface IBaseModel
    {
        Guid id { get; set; }
        string Type { get; }
        bool isDeleted { get; set; }
    }

    public class BaseModel : IBaseModel
    {
        public Guid id { get; set; }
        public virtual string Type { get; } = "Base";
        public bool isDeleted { get; set; } = false;
    }
    public class User : BaseModel
    {
        public override string Type { get; } = "User";
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
        // public List<Guid>? Friends { get; set; }
        public List<CoffeeTypePreference> CoffeeTypePreferences { get; set; }
        public List<RoastLevelPreference> RoastLevelPreferences { get; set; }
        public List<FlavorNotePreference> FlavorNotePreferences { get; set; }
        public List<OriginPreference> OriginPreferences { get; set; }
        public List<BrewingMethodPreference> BrewingMethodPreferences { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<Badge> Badges { get; set; }
    }

    public class Coffee : BaseModel
    {
        public override string Type { get; } = "Coffee";
        public string CoffeeName { get; set; }
        public string CoffeeType { get; set; }
        public string Origin { get; set; }

        public string RoastLevel { get; set; }
        public string FlavorNotes { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }

        public Roaster Roaster { get; set; }

    }

    public class Roaster : BaseModel
    {
        public override string Type { get; } = "Roaster";
        public string RoasterName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string WebsiteUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public List<Guid> RoastedCoffees { get; set; }
    }

    public class CoffeeShop : BaseModel
    {
        public override string Type { get; } = "CoffeeShop";
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


    //Visit to a CoffeeShop
    public class CheckIn : BaseModel
    {
        public override string Type { get; } = "CheckIn";
        public User User { get; set; }
        public CoffeeShop CoffeeShop { get; set; }
        public DateTime CheckinDate { get; set; }
        public List<string> CheckinPhotos { get; set; }
        public List<Coffee> Coffees { get; set; }
    }

    public class Badge : BaseModel
    {
        public override string Type { get; } = "Badge";
        public string BadgeName { get; set; }
        public string BadgeDescription { get; set; }
        public string BadgeIconUrl { get; set; }
        public string BadgeCriteria { get; set; }
    }



    public class Review : BaseModel
    {
        public override string Type { get; } = "Review";
        public Guid UserId { get; set; }
        public Guid CoffeeId { get; set; }
        // This is nullable because a review can be for a coffee, or for a coffee at a coffee shop
        public Guid? CoffeeShopId { get; set; }
        public int Rating { get; set; }
        public double NormalizedRating { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
        public List<ReviewLike> ReviewLikes { get; set; }
        public List<Comment> Comments { get; set; }
    }
    //Likes on a Review
    public class ReviewLike : BaseModel
    {
        public override string Type { get; } = "ReviewLike";
        public Guid UserId { get; set; }
        public Guid ReviewId { get; set; }
        public DateTime LikedDate { get; set; }
    }

    public class Comment : BaseModel
    {
        public override string Type { get; } = "Comment";
        public Guid UserId { get; set; }
        public Guid ReviewId { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; }
    }
    /* 
        public class UserFollowing
        {
            public Guid id { get; set; }
            public Guid FollowerId { get; set; }
            public Guid FolloweeId { get; set; }
            public DateTime FollowDate { get; set; }
        } */

    public class Notification : BaseModel
    {
        public override string Type { get; } = "Notification";
        public Guid UserId { get; set; }
        public string NotificationType { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
    }

    public class Recommendation : BaseModel
    {
        public override string Type { get; } = "Recommendation";
        public Guid UserId { get; set; }
        public Guid CoffeeId { get; set; }
        public Guid RecommendedBy { get; set; }
        public string RecommendationReason { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /*     public class CoffeeShopPhoto
        {
            public Guid id { get; set; }
            public Guid CoffeeShopId { get; set; }
            public Guid UserId { get; set; }
            public string PhotoUrl { get; set; }
            public string Description { get; set; }
            public DateTime UploadDate { get; set; }
        }

        public class CoffeePhoto
        {
            public Guid id { get; set; }
            public Guid CoffeeId { get; set; }
            public Guid UserId { get; set; }
            public string PhotoUrl { get; set; }
            public string Description { get; set; }
            public DateTime UploadDate { get; set; }
        }

        public class UserPhoto
        {
            public Guid id { get; set; }
            public Guid UserId { get; set; }
            public string PhotoUrl { get; set; }
            public string Description { get; set; }
            public DateTime UploadDate { get; set; }
        } */
    /*     public class FriendRequest
        {
            public Guid id { get; set; }
            public Guid RequesterId { get; set; }
            public Guid RecipientId { get; set; }
            public string RequestStatus { get; set; }
            public DateTime RequestDate { get; set; }
        }


        public class FollowRequest
        {
            public Guid id { get; set; }
            public Guid RequesterId { get; set; }
            public Guid RecipientId { get; set; }
            public string RequestStatus { get; set; } // "Pending", "Accepted", "Rejected", "Canceled"
            public DateTime RequestDate { get; set; }
        } */

    /*  public class Event
     {
         public Guid id { get; set; }
         public string EventName { get; set; }
         public string EventDescription { get; set; }
         public DateTime EventDate { get; set; }
         public string EventLocation { get; set; }
         public Guid CoffeeShopId { get; set; }
         public Guid OrganizerId { get; set; }

     }

     public class UserEvent
     {
         public Guid id { get; set; }
         public Guid UserId { get; set; }
         public Guid EventId { get; set; }
         public string Status { get; set; }
     } */

}
