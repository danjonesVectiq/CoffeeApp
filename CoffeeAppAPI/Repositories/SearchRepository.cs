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
        Task<IEnumerable<BaseSearchResult>> SearchAllAsync(string query, int topResults = 10);

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
            return await _searchService.SearchAsync<CoffeeSearchResult>(SearchIndexInstance.Coffee, query, topResults);
        }

        public async Task<SearchResults<CoffeeShopSearchResult>> SearchCoffeeShopsAsync(string query, int topResults)
        {
            return await _searchService.SearchAsync<CoffeeShopSearchResult>(SearchIndexInstance.CoffeeShop, query, topResults);
        }

        public async Task<SearchResults<RoasterSearchResult>> SearchRoastersAsync(string query, int topResults)
        {
            return await _searchService.SearchAsync<RoasterSearchResult>(SearchIndexInstance.Roaster, query, topResults);
        }

        public async Task<IEnumerable<BaseSearchResult>> SearchAllAsync(string query, int topResults = 10)
        {
            var coffeeTask = SearchCoffeesAsync(query, topResults);
            var coffeeShopTask = SearchCoffeeShopsAsync(query, topResults);
            var roasterTask = SearchRoastersAsync(query, topResults);

            await Task.WhenAll(coffeeTask, coffeeShopTask, roasterTask);

            var coffeeResults = coffeeTask.Result;
            var coffeeShopResults = coffeeShopTask.Result;
            var roasterResults = roasterTask.Result;

            var results = new List<BaseSearchResult>()
                .Concat(coffeeResults.GetResults().Select(r => r.Document))
                .Concat(coffeeShopResults.GetResults().Select(r => r.Document))
                .Concat(roasterResults.GetResults().Select(r => r.Document));

            return results;
        }

    }
}