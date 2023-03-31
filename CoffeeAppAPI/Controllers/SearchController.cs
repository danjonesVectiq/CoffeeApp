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
        public async Task<IActionResult> SearchCoffees(string query, int topResults = 10)
        {
            var results = await _searchRepository.SearchCoffeesAsync(query, topResults);
            return Ok(results);
        }

        [HttpGet("coffeeshops")]
        public async Task<IActionResult> SearchCoffeeShops(string query, int topResults = 10)
        {
            var results = await _searchRepository.SearchCoffeeShopsAsync(query, topResults);
            return Ok(results);
        }

        [HttpGet("roasters")]
        public async Task<IActionResult> SearchRoasters(string query, int topResults = 10)
        {
            var results = await _searchRepository.SearchRoastersAsync(query, topResults);
            return Ok(results);
        }

        [HttpGet("all")]
        public async Task<IActionResult> SearchAll(string query, int topResults = 10)
        {
            var results = await _searchRepository.SearchAllAsync(query, topResults);
            return Ok(results);
        }
    }
}
