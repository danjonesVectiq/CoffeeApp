using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.OpenApi.Services;


namespace CoffeeAppAPI.Repositories
{
public interface ISearchRepository
    {
        Task IndexDataAsync(IEnumerable<Coffee> coffees, IEnumerable<CoffeeShop> coffeeShops, IEnumerable<Roaster> roasters);
        Task<SearchResults<SearchResult>> PerformSearchAsync(string searchText, SearchOptions options);
    }
    public class SearchRepository : ISearchRepository
    {
        private readonly SearchService _searchService;

        public SearchRepository(SearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task IndexDataAsync(IEnumerable<Coffee> coffees, IEnumerable<CoffeeShop> coffeeShops, IEnumerable<Roaster> roasters)
        {
            await _searchService.IndexDataAsync();
        }

        public async Task<SearchResults<SearchResult>> PerformSearchAsync(string searchText, SearchOptions options)
        {
            return await _searchService.PerformSearchAsync(searchText, options.Filter, options.Skip, options.Size);
        }
    }
}