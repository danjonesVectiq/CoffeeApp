using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{

    public enum SearchIndexInstance
    {
        User,
        Coffee,
        Interaction
    }

    public class SearchService
    {
        private readonly Uri _serviceEndpoint;
        private readonly AzureKeyCredential _adminCredentials;

        private static readonly IReadOnlyDictionary<SearchIndexInstance, string> IndexNames = new Dictionary<SearchIndexInstance, string>
        {
            { SearchIndexInstance.User, "user-index" },
            { SearchIndexInstance.Coffee, "coffee-index" },
            { SearchIndexInstance.Interaction, "interaction-index" },
        };

        public SearchService(IConfiguration configuration)
        {
            var azureConfig = configuration.GetSection("AzureCognitiveSearch");
            _serviceEndpoint = new Uri($"https://{azureConfig["SearchServiceName"]}.search.windows.net");
            _adminCredentials = new AzureKeyCredential(azureConfig["AdminApiKey"]);
        }

        public async Task<SearchResults<T>> SearchAsync<T>(SearchIndexInstance indexInstance, string searchText, int topResults = 10, string typeFilter = null)
        {


            if (!IndexNames.TryGetValue(indexInstance, out string indexName))
            {
                throw new ArgumentException("Invalid index provided.");
            }
            Console.WriteLine($"Searching index {indexName} for {searchText}...");
            var searchClient = new SearchClient(_serviceEndpoint, indexName, _adminCredentials);
            Console.WriteLine($"SearchClient: {searchClient}");
            var searchOptions = new SearchOptions
            {
                Size = topResults,
                SearchMode = SearchMode.Any,
                IncludeTotalCount = true,
                Filter = typeFilter
            };

            Console.WriteLine($"Search query: {searchText}");
            Console.WriteLine($"Search options: {JsonConvert.SerializeObject(searchOptions)}");


            Response<SearchResults<T>> response = await searchClient.SearchAsync<T>(searchText, searchOptions);
            return response.Value;
        }
    }
}
