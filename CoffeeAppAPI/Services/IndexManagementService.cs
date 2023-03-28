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

            public async Task InitializeAsync(string dataSourceName, string indexName, string[] fieldNames)
            {
                await _searchService.CreateDataSourceAsync("coffeeds", "coffee");
                await _searchService.CreateDataSourceAsync("coffeeshopds", "coffeeShop");
                await _searchService.CreateDataSourceAsync("roasterds", "roaster");

                await _searchService.CreateIndexForContainerAsync("coffee-index", fieldNames);
                await _searchService.CreateIndexForContainerAsync("coffeeshop-index", fieldNames);
                await _searchService.CreateIndexForContainerAsync("roaster-index", fieldNames);

                await _searchService.CreateIndexerForDataSourceAsync("coffee-indexer", "coffeeds", "coffee-index");
                await _searchService.CreateIndexerForDataSourceAsync("coffeeshop-indexer", "coffeeshopds", "coffeeshop-index");
                await _searchService.CreateIndexerForDataSourceAsync("roaster-indexer", "roasterds", "roaster-index");

            }
        }
    }
}
