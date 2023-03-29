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


        Uri serviceEndpoint;
        AzureKeyCredential adminCredentials;
        string connectionString = "";


        public SearchService(IOptions<AzureCognitiveSearchSettings> settings, IConfiguration configuration)
        {

            connectionString = configuration.GetSection("CosmosDb")["ConnectionString"];
            var serviceEndpoint = new Uri($"https://{settings.Value.SearchServiceName}.search.windows.net");
            var adminCredentials = new AzureKeyCredential(settings.Value.AdminApiKey);

            _searchIndexClient = new SearchIndexClient(serviceEndpoint, adminCredentials);
            _searchIndexerClient = new SearchIndexerClient(serviceEndpoint, adminCredentials);
        }

        public async Task CreateDataSourceAsync(string dataSourceName, string containerName)
        {
            var dataSource = new SearchIndexerDataSourceConnection(dataSourceName, SearchIndexerDataSourceType.CosmosDb, connectionString, new SearchIndexerDataContainer(containerName));
            await _searchIndexerClient.CreateOrUpdateDataSourceConnectionAsync(dataSource);
        }
        public async Task RunIndexerAsync(string indexerName)
        {
            await _searchIndexerClient.RunIndexerAsync(indexerName);
        }

        public async Task<SearchIndexerStatus> GetIndexerStatusAsync(string indexerName)
        {
            return (await _searchIndexerClient.GetIndexerStatusAsync(indexerName)).Value;
        }

        public async Task CreateIndexForContainerAsync(string indexName, string[] fieldNames)
        {
            var index = new SearchIndex(indexName);

            foreach (string fieldName in fieldNames)
            {
                index.Fields.Add(new SearchableField(fieldName) { IsFilterable = true, IsSortable = true, IsFacetable = false });
            }

            await _searchIndexClient.CreateOrUpdateIndexAsync(index);
        }

        public async Task CreateIndexerForDataSourceAsync(string indexerName, string dataSourceName, string indexName)
        {
            var indexer = new SearchIndexer(indexerName, dataSourceName, indexName);
            await _searchIndexerClient.CreateOrUpdateIndexerAsync(indexer);
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
