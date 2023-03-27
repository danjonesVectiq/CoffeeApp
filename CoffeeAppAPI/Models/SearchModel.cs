namespace CoffeeAppAPI.Models
{
    public class SearchRequest
    {
        public string CoffeeName { get; set; }
        public string CoffeeShopName { get; set; }
        public string UserName { get; set; }
        public string CoffeeType { get; set; }
        public string RoastLevel { get; set; }
        public string Origin { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string FlavorNotes { get; set; }
        public string RoasterName { get; set; }
    }

}