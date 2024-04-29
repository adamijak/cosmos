using System.Net;
using Adamijak.Cosmos.Extensions;
using Adamijak.Cosmos.Queue;
using Adamijak.CosmosTest.Common;
using Microsoft.Azure.Cosmos;

namespace Adamijak.CosmosTest.Queue;

[TestClass]
public class CosmosQueueBulk
{
    
    private static Container container = null!;
    private static CosmosQueue<QueueItemPk> queue = null!;
    private const int Count = 1_000;
    
    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        var containerId = Guid.NewGuid().ToString();
        container = await AssemblyInitializer.Database.CreateContainerIfNotExistsAsync(containerId, "/id");
        queue = new CosmosQueueBuilder<QueueItemPk>(container).Build();
    }
    
    [TestMethod]
    public async Task BulkCreate()
    {
        var items = Enumerable.Range(0, Count)
            .Select(i => new QueueItemPk
            {
                Id = Guid.NewGuid().ToString(),
                Pk = nameof(BulkCreate),
            })
            .ToList();
        await items.ForEachAsync(queue.CreateAsync, 100);
        Assert.AreEqual(Count, await queue.CountAsync());
    }
    
    [TestMethod]
    public async Task BulkProcess()
    {
        var items = Enumerable.Range(0, Count)
            .Select(i => new QueueItemPk
            {
                Id = Guid.NewGuid().ToString(),
                Pk = nameof(BulkProcess),
            })
            .ToList();
        
        await items.ForEachAsync(queue.CreateAsync, 100);

        var counter = 0;
        await queue.ToAsyncEnumerable(q => q.Where(i => i.Pk == nameof(BulkProcess)))
            .ForEachAsync((i) => 
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