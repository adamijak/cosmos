using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Adamijak.CosmosTest;

[TestClass]
public class AssemblyInitializer
{
    // private static CosmosDbContainer? container;
    public static IConfiguration? Configuration;
    public static CosmosClient? CosmosClient;
    public static Database? Database;
    [AssemblyInitialize]
    public static async Task AssemblyInitialize(TestContext testContext)
    {
        Configuration = new ConfigurationBuilder()
            .AddIniFile("settings.ini")
            .Build();
        // container = new CosmosDbBuilder()
        //     .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
        //     .Build();
        // await container.StartAsync();
        // var connectionString = container.GetConnectionString();
        CosmosClient = new(Configuration["cosmos_connection_string"], new CosmosClientOptions()
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        });
        Database = await CosmosClient.CreateDatabaseIfNotExistsAsync("test-database");
    }

    [AssemblyCleanup]
    public static async Task AssemblyCleanupAsync()
    {
        if (Database is not null)
        {
            await Database.DeleteAsync();
        }
        CosmosClient?.Dispose();

        // if (container is not null)
        // {
        //     await container.DisposeAsync();
        // }
    }
}