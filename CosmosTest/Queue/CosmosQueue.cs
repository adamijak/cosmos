using Adamijak.Cosmos.Queue;
using Microsoft.Azure.Cosmos;

namespace Adamijak.CosmosTest.Queue;

[TestClass]
public class CosmosQueue
{
    
    private static Container container = null!;
    private static CosmosQueue<QueueItem> queue = null!;
    
    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        var containerId = Guid.NewGuid().ToString();
        container = await AssemblyInitializer.Database.CreateContainerIfNotExistsAsync(containerId, "/id");
        queue = new CosmosQueueBuilder<QueueItem>(container).Build();
    }
    
    private QueueItem SimpleQueueItem => new()
    {
        Id = Guid.NewGuid().ToString(),
    };
    
    [TestMethod]
    public async Task Enqueue()
    {
        var item = SimpleQueueItem;
        QueueItem result = await queue.EnqueueAsync(item);
        
        Assert.AreEqual(item.Id, result.Id);
        Assert.AreNotEqual(default, result.Id);
    }

    [TestMethod]
    public async Task ReadSystemProperties()
    {
        var item = SimpleQueueItem;
        QueueItem result = await queue.EnqueueAsync(item);
        
        Assert.AreNotEqual(default, result.ResourceId);
        Assert.AreNotEqual(default, result.Self);
        Assert.AreNotEqual(default, result.ETag);
        Assert.AreNotEqual(default, result.Attachments);
        Assert.AreNotEqual(default, result.Timestamp);
    }
    
    [TestMethod]
    public async Task EnqueueDequeue()
    {
        var item = SimpleQueueItem;
        await queue.EnqueueAsync(item);
        
        var result = await queue.DequeueAsync(item);
        Assert.AreEqual(item.Id, result.Resource.Id);
    }
    
    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        await container.DeleteContainerAsync();
    }
   
}