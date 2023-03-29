using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CoffeeAppAPI.Services;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Extensions.Configuration;

namespace CoffeeAppCLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Retrieve the SearchService and IndexManagementService instances
            var searchService = host.Services.GetRequiredService<CoffeeAppAPI.Services.SearchService>();
            var indexManagementService = host.Services.GetRequiredService<CoffeeAppAPI.Services.IndexManagementService>();

            // Parse the command-line arguments
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "initialize":
                        indexManagementService.InitializeAsync().GetAwaiter().GetResult();
                        Console.WriteLine("Initialization completed.");
                        break;
                    case "status":
                        if (args.Length > 1)
                        {
                            string indexerName = args[1];
                            var indexerStatus = indexManagementService.GetIndexerStatusAsync(indexerName).GetAwaiter().GetResult();
                            Console.WriteLine($"{indexerName} indexer status: {indexerStatus.Status}");
                        }
                        else
                        {
                            Console.WriteLine("Please provide an indexer name to check its status.");
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid argument. Use 'initialize' or 'status <indexerName>'.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Please provide an argument. Use 'initialize' or 'status <indexerName>'.");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<IConfiguration>(hostContext.Configuration);
            services.AddTransient<CoffeeAppAPI.Services.SearchService>();
            services.AddTransient<CoffeeAppAPI.Services.IndexManagementService>();
            // Register other services as needed
        });

    }
}
