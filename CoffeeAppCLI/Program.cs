using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CoffeeAppAPI.Repositories;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Extensions.Configuration;

namespace CoffeeAppCLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Retrieve the SearchRepository and IndexManagementRepository instances
            var searchRepository = host.Repositories.GetRequiredRepository<CoffeeAppAPI.Repositories.SearchRepository>();
            var indexManagementRepository = host.Repositories.GetRequiredRepository<CoffeeAppAPI.Repositories.IndexManagementRepository>();

            // Parse the command-line arguments
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "initialize":
                        indexManagementRepository.InitializeAsync().GetAwaiter().GetResult();
                        Console.WriteLine("Initialization completed.");
                        break;
                    case "status":
                        if (args.Length > 1)
                        {
                            string indexerName = args[1];
                            var indexerStatus = indexManagementRepository.GetIndexerStatusAsync(indexerName).GetAwaiter().GetResult();
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
        .ConfigureRepositories((hostContext, services) =>
        {
            services.AddSingleton<IConfiguration>(hostContext.Configuration);
            services.AddTransient<CoffeeAppAPI.Repositories.SearchRepository>();
            services.AddTransient<CoffeeAppAPI.Repositories.IndexManagementRepository>();
            // Register other services as needed
        });

    }
}
