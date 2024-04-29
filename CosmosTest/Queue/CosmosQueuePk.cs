using Adamijak.Cosmos.Queue;
using Adamijak.CosmosTest.Common;
using Microsoft.Azure.Cosmos;

namespace Adamijak.CosmosTest.Queue;

[TestClass]
public class CosmosQueuePk
{
    
    private static Container container = null!;
    private static CosmosQueue<QueueItemPk> queue = null!;
    
    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        var containerId = Guid.NewGuid().ToString();
        container = await AssemblyInitializer.Database.CreateContainerIfNotExistsAsync(containerId, "/pk");
        queue = new CosmosQueueBuilder<QueueItemPk>(container)
            .WithPartitionKey(i => new PartitionKey(i.Pk))
            .Build();
    }
    
    private QueueItemPk SimpleQueueItem => new()
    {
        Id = Guid.NewGuid().ToString(),
        Pk = Guid.NewGuid().ToString(),
    };
    
    [TestMethod]
    public async Task Create()
    {
        var item = SimpleQueueItem;
        QueueItemPk result = await queue.CreateAsync(item);
        Assert.AreEqual(item.Id, result.Id);
        Assert.AreEqual(item.Pk, result.Pk);
    }
    
    [TestMethod]
    public async Task CreateDelete()
    {
        var item = SimpleQueueItem;
        await queue.CreateAsync(item);
        
        var result = await queue.ReadDeleteAsync(item);
        Assert.AreEqual(item.Id, result.Resource.Id);
        Assert.AreEqual(item.Pk, result.Resource.Pk);
    }
    
    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        await container.DeleteContainerAsync();
    }
   
}