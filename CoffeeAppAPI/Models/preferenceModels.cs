namespace CoffeeAppAPI.Models
{

    public class UserPreferences
    {
        public List<string> CoffeeTypePreferences { get; set; }
        public List<string> RoastLevelPreferences { get; set; }
        public List<string> FlavorNotePreferences { get; set; }
        public List<string> OriginPreferences { get; set; }
        public List<string> BrewingMethodPreferences { get; set; }
    }

   /*  public class CoffeeTypePreference
    {
        public Guid id { get; set; }
        public string CoffeeType { get; set; }
    }

    public class RoastLevelPreference
    {
        public Guid id { get; set; }
        public string RoastLevel { get; set; }
    }

    public class FlavorNotePreference
    {
        public Guid id { get; set; }
        public string FlavorNote { get; set; }
    }

    public class OriginPreference
    {
        public Guid id { get; set; }
        public string Origin { get; set; }
    }

    public class BrewingMethodPreference
    {
        public Guid id { get; set; }
        public string BrewingMethod { get; set; }
    } */
}