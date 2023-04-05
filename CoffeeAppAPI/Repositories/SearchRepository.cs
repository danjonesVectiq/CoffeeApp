using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public interface ISearchRepository
    {
        Task<SearchResults<CoffeeSearchResult>> SearchCoffeesAsync(string query, int topResults = 10);
        Task<SearchResults<CoffeeShopSearchResult>> SearchCoffeeShopsAsync(string query, int topResults = 10);
        Task<SearchResults<RoasterSearchResult>> SearchRoastersAsync(string query, int topResults = 10);
        Task<CombinedSearchResult> SearchAllAsync(string query, int topResults = 10);

    }

    public class SearchRepository : ISearchRepository
    {
        private readonly SearchService _searchService;

        public SearchRepository(SearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<SearchResults<CoffeeSearchResult>> SearchCoffeesAsync(string query, int topResults)
        {
            return await _searchService.SearchAsync<CoffeeSearchResult>(SearchIndexInstance.Coffee, query, topResults, "Type eq 'Coffee'");
        }

        public async Task<SearchResults<CoffeeShopSearchResult>> SearchCoffeeShopsAsync(string query, int topResults)
        {
            return await _searchService.SearchAsync<CoffeeShopSearchResult>(SearchIndexInstance.Coffee, query, topResults, "Type eq 'CoffeeShop'");
        }

        public async Task<SearchResults<RoasterSearchResult>> SearchRoastersAsync(string query, int topResults)
        {
            return await _searchService.SearchAsync<RoasterSearchResult>(SearchIndexInstance.Coffee, query, topResults, "Type eq 'Roaster'");
        }

        public async Task<CombinedSearchResult> SearchAllAsync(string query, int topResults)
        {
            var coffees = await SearchCoffeesAsync(query, topResults);
            var coffeeShops = await SearchCoffeeShopsAsync(query, topResults);
            var roasters = await SearchRoastersAsync(query, topResults);

            return new CombinedSearchResult
            {
                Coffees = coffees.GetResults().Select(r => r.Document),
                CoffeeShops = coffeeShops.GetResults().Select(r => r.Document),
                Roasters = roasters.GetResults().Select(r => r.Document)
            };


        }
    }
}