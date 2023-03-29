using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using CoffeeAppAPI.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Services;
using System;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    using System.Threading.Tasks;

    namespace CoffeeAppAPI.Services
    {
        public class IndexManagementService
        {
            private readonly SearchService _searchService;

            public IndexManagementService(SearchService searchService)
            {
                _searchService = searchService;
            }

            public async Task InitializeAsync()
            {
                
                await _searchService.CreateDataSourceAsync("coffeeds", "coffee");
                await _searchService.CreateDataSourceAsync("coffeeshopds", "coffeeShop");
                await _searchService.CreateDataSourceAsync("roasterds", "roaster");

                string[] coffeeFieldNames = new string[] { "id", "name", "cofeetype" }; // Customize field names as needed
                string[] coffeeShopFieldNames = new string[] { "id", "name", "city" }; // Customize field names as needed
                string[] roasterFieldNames = new string[] { "id", "name", "city" }; // Customize field names as needed

                await _searchService.CreateIndexForContainerAsync("coffee-index", coffeeFieldNames);
                await _searchService.CreateIndexForContainerAsync("coffeeshop-index", coffeeShopFieldNames);
                await _searchService.CreateIndexForContainerAsync("roaster-index", roasterFieldNames);

                await _searchService.CreateIndexerForDataSourceAsync("coffee-indexer", "coffeeds", "coffee-index");
                await _searchService.CreateIndexerForDataSourceAsync("coffeeshop-indexer", "coffeeshopds", "coffeeshop-index");
                await _searchService.CreateIndexerForDataSourceAsync("roaster-indexer", "roasterds", "roaster-index");

                await _searchService.RunIndexerAsync("coffee-indexer");
                await _searchService.RunIndexerAsync("coffeeshop-indexer");
                await _searchService.RunIndexerAsync("roaster-indexer");

            }

            public async Task<SearchIndexerStatus> GetIndexerStatusAsync(string indexerName)
{
            return (await _searchService.GetIndexerStatusAsync(indexerName));
}
        }
    }
}
