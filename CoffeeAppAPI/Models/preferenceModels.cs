namespace CoffeeAppAPI.Models
{
    public class UserPreferences
    {
        public List<CoffeeTypePreference> CoffeeTypePreferences { get; set; }
        public List<RoastLevelPreference> RoastLevelPreferences { get; set; }
        public List<FlavorNotePreference> FlavorNotePreferences { get; set; }
        public List<OriginPreference> OriginPreferences { get; set; }
        public List<BrewingMethodPreference> BrewingMethodPreferences { get; set; }
    }


    public class CoffeeTypePreference
    {
        public Guid id { get; set; }
        public string CoffeeType { get; set; }
        public int Importance { get; set; } // Rating from 1 to 5
    }

    public class RoastLevelPreference
    {
        public Guid id { get; set; }
        public string RoastLevel { get; set; }
        public int Importance { get; set; } // Rating from 1 to 5
    }

    public class FlavorNotePreference
    {
        public Guid id { get; set; }
        public string FlavorNote { get; set; }
        public int Importance { get; set; } // Rating from 1 to 5
    }

    public class OriginPreference
    {
        public Guid id { get; set; }
        public string Origin { get; set; }
        public int Importance { get; set; } // Rating from 1 to 5
    }

    public class BrewingMethodPreference
    {
        public Guid id { get; set; }
        public string BrewingMethod { get; set; }
        public int Importance { get; set; } // Rating from 1 to 5
    }
}