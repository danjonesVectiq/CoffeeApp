using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using CoffeeAppAPI.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Services;
using System;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{

    public class SearchService
    {
        private readonly SearchClient _searchClient;
        private readonly SearchIndexClient _searchIndexClient;
        private readonly SearchIndexerClient _searchIndexerClient;

        IConfigurationSection azureConfig;
        Uri serviceEndpoint;
        AzureKeyCredential adminCredentials;
        string connectionString = "";
        public SearchService(IConfiguration configuration)
        {
            //IOptions<AzureCognitiveSearchSettings> settings, maybe use this later in the constructor.

            azureConfig = configuration.GetSection("AzureCognitiveSearch");
            var cosmosDbConfig = configuration.GetSection("CosmosDb");
            connectionString = cosmosDbConfig["ConnectionString"] + "Database=CoffeeApp";

            Console.WriteLine($"AdminApiKey: {connectionString}");

            Console.WriteLine($"SearchServiceName: {azureConfig["SearchServiceName"]}");
            Console.WriteLine($"AdminApiKey: {azureConfig["AdminApiKey"]}");

            serviceEndpoint = new Uri($"https://{azureConfig["SearchServiceName"]}.search.windows.net");


            adminCredentials = new AzureKeyCredential(azureConfig["AdminApiKey"]);

            _searchIndexClient = new SearchIndexClient(serviceEndpoint, adminCredentials);
            _searchIndexerClient = new SearchIndexerClient(serviceEndpoint, adminCredentials);
        }

       


        public async Task<SearchResults<SearchResult>> PerformSearchAsync(string searchText, string indexName, string searchFilter = null, int? skip = null, int? take = null, string[] fieldNames = null)
        {
            var searchClient = new SearchClient(serviceEndpoint, indexName, adminCredentials);

            var options = new SearchOptions
            {
                Filter = searchFilter,
                Skip = skip,
                Size = take,
            };

            if (fieldNames != null)
            {
                foreach (string fieldName in fieldNames)
                {
                    options.Select.Add(fieldName);
                }
            }

            Response<SearchResults<SearchResult>> response = await searchClient.SearchAsync<SearchResult>(searchText, options);
            return response.Value;
        }

    }
}
