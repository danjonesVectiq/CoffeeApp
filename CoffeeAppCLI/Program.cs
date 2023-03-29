using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CoffeeAppAPI.Services;
using CoffeeAppAPI.Services.CoffeeAppAPI.Services;
using CoffeeAppAPI.Configuration;
using Azure.Search.Documents.Indexes.Models;

namespace CoffeeAppCLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set up dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            using var serviceProvider = services.BuildServiceProvider();

            // Get the IndexManagementService instance
            var indexManagementService = serviceProvider.GetRequiredService<IndexManagementService>();

            // Check command line arguments
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a command: initialize or status");
                return;
            }

            string command = args[0];

            switch (command.ToLower())
            {
                case "initialize":
                    string[] fieldNames = new string[] { "id", "name", "description" }; // Customize field names as needed
                    await indexManagementService.InitializeAsync();
                    Console.WriteLine("Initialization completed.");
                    break;
                case "status":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Please provide an indexer name for the status command.");
                        return;
                    }
                    string indexerName = args[1];
                    SearchIndexerStatus status = await indexManagementService.GetIndexerStatusAsync(indexerName);
                    Console.WriteLine($"Indexer {indexerName} status: {status.Status}");
                    break;

                default:
                    Console.WriteLine("Unknown command. Please use 'initialize' or 'status'");
                    break;
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Configure the AzureCognitiveSearchSettings object
            services.Configure<AzureCognitiveSearchSettings>(options =>
            {
                options.SearchServiceName = "your_search_service_name";
                options.AdminApiKey = "your_admin_api_key";
            });

            // Add the SearchService and IndexManagementService
            services.AddSingleton<SearchService>();
            services.AddSingleton<IndexManagementService>();
        }
    }
}
