using CoffeeAppAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Azure.Search.Documents.Indexes.Models;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndexManagementController : ControllerBase
    {
        private readonly IndexManagementService _indexManagementService;

        public IndexManagementController(IndexManagementService indexManagementService)
        {
            _indexManagementService = indexManagementService;
        }

        [HttpGet("initialize")]
        public async Task<IActionResult> InitializeAsync()
        {
            await _indexManagementService.InitializeAsync();
            return Ok("Initialization completed.");
        }

        [HttpGet("status/{indexerName}")]
        public async Task<IActionResult> GetIndexerStatusAsync(string indexerName)
        {
            SearchIndexerStatus indexerStatus = await _indexManagementService.GetIndexerStatusAsync(indexerName);
            return Ok($"{indexerName} indexer status: {indexerStatus.Status}");
        }
    }
}
