namespace CoffeeAppAPI.Models
{
    public class Rating :BaseModel
    {
        public int RatingValue { get; set; }
        public string UserId { get; set; }
    }
    public class CoffeeRating : Rating
    {
        public string Type { get; } = "CoffeeRating";
        public Guid CoffeeId { get; set; }
    }
    public class CoffeeShopRating : Rating
    {
         public string Type { get; } = "CoffeeShopRating";
        public Guid CoffeeShopId { get; set; }
    }
    public class RecipeRating : Rating
    {
        public string Type { get; } = "RecipeRating";
        public Guid RecipeId { get; set; }
    }
}