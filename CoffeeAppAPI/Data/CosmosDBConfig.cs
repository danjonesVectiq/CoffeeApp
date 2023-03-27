namespace CoffeeAppAPI.Configuration
{
    public class CosmosDbConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public class AzureCognitiveSearchSettings
    {
        public string SearchServiceName { get; set; }
        public string AdminApiKey { get; set; }
    }
}
