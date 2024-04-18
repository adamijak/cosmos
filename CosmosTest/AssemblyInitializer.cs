using Microsoft.Azure.Cosmos;
[assembly: Parallelize(Workers = 4, Scope = ExecutionScope.ClassLevel)]

namespace Adamijak.CosmosTest;

[TestClass]
public class AssemblyInitializer
{
    public static CosmosClient CosmosClient = null!;
    public static Database Database = null!;
    [AssemblyInitialize]
    public static async Task AssemblyInitialize(TestContext testContext)
    {
        CosmosClient = new(Environment.GetEnvironmentVariable("COSMOS_CS"), new CosmosClientOptions()
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        });
        Database = await CosmosClient.CreateDatabaseIfNotExistsAsync(Default.DatabaseId);
    }

    [AssemblyCleanup]
    public static async Task AssemblyCleanupAsync()
    {
        await Database.DeleteAsync();
        CosmosClient.Dispose();
    }
}