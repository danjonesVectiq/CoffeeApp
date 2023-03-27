using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using CoffeeAppAPI.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeAppAPI.Configuration;
using Microsoft.OpenApi.Services;

namespace CoffeeAppAPI.Services
{
    public class SearchService
    {
        private readonly SearchClient _searchClient;
        private readonly SearchIndexClient _searchIndexClient;

        public SearchService(IOptions<AzureCognitiveSearchSettings> settings)
        {
            var serviceEndpoint = new Uri($"https://{settings.Value.SearchServiceName}.search.windows.net");
            var adminCredentials = new AzureKeyCredential(settings.Value.AdminApiKey);
            _searchClient = new SearchClient(serviceEndpoint, "coffeesearchindex", adminCredentials);
            _searchIndexClient = new SearchIndexClient(serviceEndpoint, adminCredentials);
        }
        public async Task CreateSearchIndexAsync()
        {
            var index = new SearchIndex("coffeesearchindex")
            {
                Fields =
                {
                    new SimpleField("id", SearchFieldDataType.String) { IsKey = true, IsFilterable = true, IsSortable = true, IsFacetable = false },
                    new SearchableField("name") { IsFilterable = true, IsSortable = true, IsFacetable = false },
                    new SearchableField("type") { IsFilterable = true, IsSortable = true, IsFacetable = false },
                    // Add more fields as needed for coffee shops, roasters, and coffees
                }
            };

            await _searchIndexClient.CreateOrUpdateIndexAsync(index);
        }

        public async Task IndexDataAsync(IEnumerable<Coffee> coffees, IEnumerable<CoffeeShop> coffeeShops, IEnumerable<Roaster> roasters)
        {
            var documents = new List<SearchDocument>();

            // Convert coffees, coffeeShops, and roasters to search documents
            documents.AddRange(coffees.Select(ConvertCoffeeToSearchDocument));
            documents.AddRange(coffeeShops.Select(ConvertCoffeeShopToSearchDocument));
            documents.AddRange(roasters.Select(ConvertRoasterToSearchDocument));

            var response = await _searchClient.UploadDocumentsAsync(documents);
        }

        public async Task<SearchResults<SearchResult>> PerformSearchAsync(string searchText, string searchFilter = null, int? skip = null, int? take = null)
        {
            var options = new SearchOptions
            {
                Filter = searchFilter,
                Skip = skip,
                Size = take,
            };

            options.Select.Add("id");
            options.Select.Add("name");
            options.Select.Add("type");

            Response<SearchResults<SearchResult>> response = await _searchClient.SearchAsync<SearchResult>(searchText, options);
            return response.Value;
        }

        private SearchDocument ConvertCoffeeToSearchDocument(Coffee coffee)
        {
            return new SearchDocument
    {
        {"id", $"coffee-{coffee.id}"},
        {"name", coffee.CoffeeName},
        {"type", "coffee"}
    };
        }

        private SearchDocument ConvertCoffeeShopToSearchDocument(CoffeeShop coffeeShop)
        {
            return new SearchDocument
    {
        {"id", $"coffeeshop-{coffeeShop.id}"},
        {"name", coffeeShop.CoffeeShopName},
        {"type", "coffeeShop"}
    };
        }

        private SearchDocument ConvertRoasterToSearchDocument(Roaster roaster)
        {
            return new SearchDocument
    {
        {"id", $"roaster-{roaster.id}"},
        {"name", roaster.RoasterName},
        {"type", "roaster"}
    };
        }
    }
}
