using System.Net;
using Adamijak.Cosmos.Extensions;
using Adamijak.Cosmos.Queue;
using Microsoft.Azure.Cosmos;

namespace Adamijak.CosmosTest.Queue;

[TestClass]
public class CosmosQueueBulk
{
    
    private static Container container = null!;
    private static CosmosQueue<QueueItem> queue = null!;
    private const int Count = 1_000;
    
    private static QueueItem SimpleQueueItem => new()
    {
        Id = Guid.NewGuid().ToString(),
    };
    
    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        var containerId = Guid.NewGuid().ToString();
        container = await AssemblyInitializer.Database.CreateContainerIfNotExistsAsync(containerId, "/id");
        queue = new CosmosQueueBuilder<QueueItem>(container).Build();
    }
    
    [TestMethod]
    public async Task BulkEnqueue()
    {
        var items = Enumerable.Range(0, Count)
            .Select(i => SimpleQueueItem)
            .ToList();
        await items.ForEachAsync(queue.EnqueueAsync, 100);
        Assert.AreEqual(Count, await queue.CountAsync());
    }
    
    [TestMethod]
    public async Task BulkProcess()
    {
        var items = Enumerable.Range(0, Count)
            .Select(i => SimpleQueueItem)
            .ToList();
        
        await items.ForEachAsync(queue.EnqueueAsync, 100);

        var counter = 0;
        await queue.ToAsyncEnumerable().ForEachAsync((i) =>
        {
            Interlocked.Increment(ref counter);
            return Task.CompletedTask;
        }, 100);
       
        Assert.AreEqual(Count, counter);
    }
    
    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        await container.DeleteContainerAsync();
    }
   
}