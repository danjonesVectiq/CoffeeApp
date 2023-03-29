/* using Microsoft.AspNetCore.Mvc;
using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using CoffeeAppAPI.Repositories;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Controllers
{
    //move this later
    public class IndexDataRequest
    {
        public IEnumerable<Coffee> Coffees { get; set; }
        public IEnumerable<CoffeeShop> CoffeeShops { get; set; }
        public IEnumerable<Roaster> Roasters { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchRepository _searchRepository;

        public SearchController(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        // This is an example endpoint to trigger indexing data. You might want to handle this differently in your application.
        [HttpPost("index")]
        public async Task<IActionResult> IndexData([FromBody] IndexDataRequest request)
        {
            await _searchRepository.IndexDataAsync(request.Coffees, request.CoffeeShops, request.Roasters);
            return Ok();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchText, [FromQuery] SearchOptions options)
        {
            var results = await _searchRepository.PerformSearchAsync(searchText, options);
            return Ok(results);
        }
    }
}
 */