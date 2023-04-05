using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using System.Collections.Generic;

namespace CoffeeAppAPI.Models
{

    public class BaseSearchResult
    { }


    public class UserSearchResult : BaseSearchResult
    {
        [SimpleField(IsKey = true, IsFilterable = true)]
        public Guid id { get; set; }

        [SimpleField(IsFilterable = true)]
        public string Type { get; set; }

        [SearchableField(IsSortable = true)]
        public string Username { get; set; }

        // Exclude Email and Password from the search index as they might contain sensitive information

        public static string[] GetFieldNames()
        {
            return new[]
            {
            nameof(Type),
            nameof(Username),

            //nameof(FlavorNotes)
        };
        }
    }



    public class CoffeeSearchResult : BaseSearchResult
    {
        [SimpleField(IsKey = true, IsFilterable = true)]
        public Guid id { get; set; }

        [SearchableField(IsSortable = true)]
        public string CoffeeName { get; set; }

        [SimpleField(IsFilterable = true)]
        public string Type { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public string CoffeeType { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public string Origin { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public string RoastLevel { get; set; }

        /*  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
         public string[] FlavorNotes { get; set; } */

        /*  [SimpleField(IsFilterable = true)]
         public Roaster Roaster { get; set; } */

        public static string[] GetFieldNames()
        {
            return new[]
            {
                nameof(Type),
            nameof(CoffeeName),
            nameof(CoffeeType),
            nameof(Origin),
            nameof(RoastLevel)//,
            //nameof(FlavorNotes)
        };

        }
    }
    public class RoasterSearchResult : BaseSearchResult
    {
        [SimpleField(IsKey = true, IsFilterable = true)]
        public Guid id { get; set; }
        [SimpleField(IsFilterable = true)]
        public string Type { get; set; }

        [SearchableField(IsSortable = true)]
        public string RoasterName { get; set; }

        [SearchableField]
        public string Address { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public string City { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public string State { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public string Country { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public double Latitude { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public double Longitude { get; set; }

        [SearchableField]
        public string Description { get; set; }


        public static string[] GetFieldNames()
        {
            return new[]
            {
                nameof(Type),
            nameof(RoasterName),
            nameof(Address),
            nameof(City),
            nameof(State),
            nameof(Country),
            nameof(Latitude),
            nameof(Longitude),
            nameof(Description)
        };

        }
    }

    public class CoffeeShopSearchResult : BaseSearchResult
    {
        [SimpleField(IsKey = true, IsFilterable = true)]
        public Guid id { get; set; }
        [SimpleField(IsFilterable = true)]
        public string Type { get; set; }

        [SearchableField(IsSortable = true)]
        public string CoffeeShopName { get; set; }

        [SearchableField]
        public string Address { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public string City { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public string State { get; set; }

        [SearchableField(IsFilterable = true, IsFacetable = true)]
        public string Country { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public double Latitude { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public double Longitude { get; set; }

        public static string[] GetFieldNames()
        {
            return new[]
            {
                nameof(Type),
            nameof(CoffeeShopName),
            nameof(Address),
            nameof(City),
            nameof(State),
            nameof(Country),
            nameof(Latitude),
            nameof(Longitude)
        };
        }

    }
}