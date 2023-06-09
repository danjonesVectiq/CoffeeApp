using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public interface ISearchService
    {
        Task<SearchResults<CoffeeSearchResult>> SearchCoffeesAsync(string query, int topResults = 10);
        Task<SearchResults<CoffeeShopSearchResult>> SearchCoffeeShopsAsync(string query, int topResults = 10);
        Task<SearchResults<RoasterSearchResult>> SearchRoastersAsync(string query, int topResults = 10);
        Task<CombinedSearchResult> SearchAllAsync(string query, int topResults = 10);
        Task<SearchResults<CoffeeShopSearchResult>> SearchCoffeeShopsNearbyAsync(double latitude, double longitude, double radius, int topResults = 10);
    }

    public class SearchService : ISearchService
    {
        private readonly SearchRepository _searchRepository;
        public SearchService(SearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }
        public async Task<SearchResults<CoffeeShopSearchResult>> SearchCoffeeShopsNearbyAsync(double latitude, double longitude, double radius, int topResults)
        {
            var geoFilter = $"geo.distance(Location, geography'POINT({longitude} {latitude})') le {radius}";
            var typeFilter = "Type eq 'CoffeeShop'";

            return await _searchRepository.SearchAsync<CoffeeShopSearchResult>(SearchIndexInstance.Coffee, "*", topResults, $"{typeFilter} and {geoFilter}");
        }

        public async Task<SearchResults<CoffeeSearchResult>> SearchCoffeesAsync(string query, int topResults)
        {
            return await _searchRepository.SearchAsync<CoffeeSearchResult>(SearchIndexInstance.Coffee, query, topResults, "Type eq 'Coffee'");
        }
        public async Task<SearchResults<CoffeeShopSearchResult>> SearchCoffeeShopsAsync(string query, int topResults)
        {
            return await _searchRepository.SearchAsync<CoffeeShopSearchResult>(SearchIndexInstance.Coffee, query, topResults, "Type eq 'CoffeeShop'");
        }
        public async Task<SearchResults<RoasterSearchResult>> SearchRoastersAsync(string query, int topResults)
        {
            return await _searchRepository.SearchAsync<RoasterSearchResult>(SearchIndexInstance.Coffee, query, topResults, "Type eq 'Roaster'");
        }
        public async Task<SearchResults<T>> SearchTypeAsync<T>(SearchIndexInstance index, string query, int topResults, string typeFilter = null)
        {
            return await _searchRepository.SearchAsync<T>(index, query, topResults, typeFilter);
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