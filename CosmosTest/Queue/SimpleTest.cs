using Adamijak.Cosmos.Queue;
using Microsoft.Azure.Cosmos;

namespace Adamijak.CosmosTest.Queue;

[TestClass]
public class SimpleTest
{
    private static Container container;
    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        container = await AssemblyInitializer.Database.CreateContainerIfNotExistsAsync(new ContainerProperties
        {
            Id = "container",
            PartitionKeyPath = "/id",
        });
    }
    
    [TestMethod]
    public async Task CreateReadTest()
    {
        var requestItem = new QueueItem()
        {
            Id = "10"
        };
        await container.CreateItemAsync(requestItem, new PartitionKey(requestItem.Id));

        var response = await container.ReadItemAsync<QueueItem>(requestItem.Id, new PartitionKey(requestItem.Id));
        
        Assert.AreEqual(requestItem.Id, response.Resource.Id);
    }
    
    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        if (container is not null)
        {
            await container.DeleteContainerAsync();
        }
    }
}