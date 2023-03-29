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
       
        Task<SearchResults<SearchResult>> PerformSearchAsync(string searchText, SearchOptions options);
    }
    public class SearchRepository : ISearchRepository
    {
        private readonly SearchService _searchService;

        public SearchRepository(SearchService searchService)
        {
            _searchService = searchService;
        }



        public async Task<SearchResults<SearchResult>> PerformSearchAsync(string searchText, SearchOptions options)
        {
            return await _searchService.PerformSearchAsync(searchText,  options.Skip, options.Size);
        }
    }
}