using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using System.Collections.Generic;

namespace CoffeeAppAPI.Models
{

    public class BaseSearchResult
    {
        [SimpleField(IsKey = true, IsFilterable = true)]
        public string id { get; set; }
        [SimpleField(IsFilterable = true)]
        public string Type { get; set; }
        public Boolean IsDeleted { get; set; }

    }


    public class UserSearchResult : BaseSearchResult
    {

        [SearchableField(IsSortable = true)]
        public string Username { get; set; }
        
    }



    public class CoffeeSearchResult : BaseSearchResult
    {

        [SearchableField(IsSortable = true)]
        public string CoffeeName { get; set; }

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


    }
    public class RoasterSearchResult : BaseSearchResult
    {
  
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
        public double? Latitude { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public double? Longitude { get; set; }

        [SearchableField]
        public string Description { get; set; }

    }

    public class CoffeeShopSearchResult : BaseSearchResult
    {

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
        public double? Latitude { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public double? Longitude { get; set; }


    }

   public class CombinedSearchResult
{
    public IEnumerable<CoffeeSearchResult> Coffees { get; set; }
    public IEnumerable<CoffeeShopSearchResult> CoffeeShops { get; set; }
    public IEnumerable<RoasterSearchResult> Roasters { get; set; }
}
}