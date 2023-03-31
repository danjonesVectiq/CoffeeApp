using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using CoffeeAppAPI.Configuration;
using CoffeeAppAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Services;
using System;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public class IndexManagementService
    {
       
        private readonly SearchIndexClient _searchIndexClient;
        private readonly SearchIndexerClient _searchIndexerClient;

        IConfigurationSection azureConfig;
        Uri serviceEndpoint;
        AzureKeyCredential adminCredentials;
        string connectionString = "";

        public IndexManagementService(IConfiguration configuration)
        {
            azureConfig = configuration.GetSection("AzureCognitiveSearch");
            var cosmosDbConfig = configuration.GetSection("CosmosDb");
            connectionString = cosmosDbConfig["ConnectionString"] + "Database=CoffeeApp";

            serviceEndpoint = new Uri($"https://{azureConfig["SearchServiceName"]}.search.windows.net");

            adminCredentials = new AzureKeyCredential(azureConfig["AdminApiKey"]);

            _searchIndexClient = new SearchIndexClient(serviceEndpoint, adminCredentials);
            _searchIndexerClient = new SearchIndexerClient(serviceEndpoint, adminCredentials);
        }

        public async Task InitializeAsync()
        {

            await CreateDataSourceAsync("coffeeds", "Coffees");
            await CreateDataSourceAsync("coffeeshopds", "CoffeeShops");
            await CreateDataSourceAsync("roasterds", "Roasters");

            string[] coffeeFieldNames = CoffeeSearchResult.GetFieldNames();
            string[] coffeeShopFieldNames = CoffeeShopSearchResult.GetFieldNames(); // Customize field names as needed
            string[] roasterFieldNames = RoasterSearchResult.GetFieldNames(); // Customize field names as needed

            await CreateIndexForContainerAsync("coffee-index", coffeeFieldNames, "id");
            await CreateIndexForContainerAsync("coffeeshop-index", coffeeShopFieldNames, "id");
            await CreateIndexForContainerAsync("roaster-index", roasterFieldNames, "id");

            await CreateIndexerForDataSourceAsync("coffee-indexer", "coffeeds", "coffee-index");
            await CreateIndexerForDataSourceAsync("coffeeshop-indexer", "coffeeshopds", "coffeeshop-index");
            await CreateIndexerForDataSourceAsync("roaster-indexer", "roasterds", "roaster-index");

            await RunIndexerAsync("coffee-indexer");
            await RunIndexerAsync("coffeeshop-indexer");
            await RunIndexerAsync("roaster-indexer");

        }

        public async Task<SearchIndexerStatus> GetIndexerStatusAsync(string indexerName)
        {
            return (await _searchIndexerClient.GetIndexerStatusAsync(indexerName)).Value;
        }

        public async Task CreateDataSourceAsync(string dataSourceName, string containerName)
        {

            Console.WriteLine($"dsn: {dataSourceName}");
            Console.WriteLine($"cs: {connectionString}");
            Console.WriteLine($"cn: {containerName}");

            var dataSource = new SearchIndexerDataSourceConnection(dataSourceName, SearchIndexerDataSourceType.CosmosDb, connectionString, new SearchIndexerDataContainer(containerName));
            await _searchIndexerClient.CreateOrUpdateDataSourceConnectionAsync(dataSource);
        }
        public async Task RunIndexerAsync(string indexerName)
        {
            await _searchIndexerClient.RunIndexerAsync(indexerName);
        }



        public async Task CreateIndexForContainerAsync(string indexName, string[] fieldNames, string keyFieldName)
        {
            var fields = fieldNames.Select(fieldName => new SearchField(fieldName, SearchFieldDataType.String)).ToList();

            // Add the key field
            fields.Add(new SearchField(keyFieldName, SearchFieldDataType.String) { IsKey = true });

            var index = new SearchIndex(indexName, fields);
            await _searchIndexClient.CreateOrUpdateIndexAsync(index);
        }

        public async Task CreateIndexerForDataSourceAsync(string indexerName, string dataSourceName, string indexName)
        {
            var indexer = new SearchIndexer(indexerName, dataSourceName, indexName);
            await _searchIndexerClient.CreateOrUpdateIndexerAsync(indexer);
        }
    }
}
