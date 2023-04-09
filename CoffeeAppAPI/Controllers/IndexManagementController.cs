using CoffeeAppAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Azure.Search.Documents.Indexes.Models;

namespace CoffeeAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndexManagementController : ControllerBase
    {
        private readonly IndexManagementRepository _indexManagementRepository;

        public IndexManagementController(IndexManagementRepository indexManagementRepository)
        {
            _indexManagementRepository = indexManagementRepository;
        }

        [HttpGet("initialize")]
        public async Task<IActionResult> InitializeAsync()
        {
            await _indexManagementRepository.InitializeAsync();
            return Ok("Initialization completed.");
        }

        [HttpGet("status/{indexerName}")]
        public async Task<IActionResult> GetIndexerStatusAsync(string indexerName)
        {
            SearchIndexerStatus indexerStatus = await _indexManagementRepository.GetIndexerStatusAsync(indexerName);
            return Ok($"{indexerName} indexer status: {indexerStatus.Status}");
        }
    }
}
