using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Adamijak.CosmosTest;

[TestClass]
public class AssemblyInitializer
{
    public static IConfiguration? Configuration;
    public static CosmosClient? CosmosClient;
    public static Database? Database;
    [AssemblyInitialize]
    public static async Task AssemblyInitialize(TestContext testContext)
    {
        Configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddIniFile("settings.ini")
            .Build();
        CosmosClient = new(Configuration.GetValue<string>("COSMOS_CONNECTION_STRING"), new CosmosClientOptions()
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
    }
}