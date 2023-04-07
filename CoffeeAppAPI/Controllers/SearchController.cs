using CoffeeAppAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchRepository _searchRepository;
        public SearchController(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }
        [HttpGet("coffees")]
        public async Task<IActionResult> SearchCoffees([FromQuery] string query, int topResults = 10)
        {
            var results = await _searchRepository.SearchCoffeesAsync(query, topResults);
            return Ok(new
            {
                totalCount = results.TotalCount,
                results = results.GetResults().Select(r => r.Document) // Add this line to include the search results in the response
            });
        }
        [HttpGet("coffeeshops")]
        public async Task<IActionResult> SearchCoffeeShops(string query, int topResults = 10)
        {
            var results = await _searchRepository.SearchCoffeeShopsAsync(query, topResults);
            return Ok(new
            {
                totalCount = results.TotalCount,
                results = results.GetResults().Select(r => r.Document) // Add this line to include the search results in the response
            });
        }
        [HttpGet("coffeeshops/nearby")]
        public async Task<IActionResult> SearchCoffeeShopsNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radius,
        [FromQuery] int topResults = 10)
        {
            
                var results = await _searchRepository.SearchCoffeeShopsNearbyAsync(latitude, longitude, radius, topResults);
                return Ok(new
                {
                    totalCount = results.TotalCount,
                    results = results.GetResults().Select(r => r.Document)
                });
           
        }

        [HttpGet("roasters")]
        public async Task<IActionResult> SearchRoasters(string query, int topResults = 10)
        {
            var results = await _searchRepository.SearchRoastersAsync(query, topResults);
            return Ok(new
            {
                totalCount = results.TotalCount,
                results = results.GetResults().Select(r => r.Document) // Add this line to include the search results in the response
            });
        }
        [HttpGet("all")]
        public async Task<IActionResult> SearchAll(string query, int topResults = 10)
        {
            var results = await _searchRepository.SearchAllAsync(query, topResults);
            return Ok(results);
        }
    }
}
