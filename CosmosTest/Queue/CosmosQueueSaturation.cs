using System.Net.Quic;
using Adamijak.Cosmos.Queue;
using Microsoft.Azure.Cosmos;

namespace Adamijak.CosmosTest.Queue;

[TestClass]
public class CosmosQueueSaturation
{
    private Container container = null!;
    
    [TestInitialize]
    public async Task TestInitialize()
    {
        var containerId = Guid.NewGuid().ToString();
        container = await AssemblyInitializer.Database.CreateContainerIfNotExistsAsync(containerId, "/id");
    }

    [TestMethod]
    [DataRow(3, 0)]
    [DataRow(3, 1)]
    [DataRow(3, 3)]
    [DataRow(3, 5)]
    [DataRow(-1, 0)]
    [DataRow(-1, 1)]
    [DataRow(-1, 3)]
    public async Task IsSaturated(int saturation, int itemCount)
    {
        var queue = new CosmosQueueBuilder<QueueItem>(container)
            .WithSaturationThreshold(saturation)
            .Build();

        for (var i = 0; i < itemCount; i++)
        {
            await queue.EnqueueAsync(new QueueItem { Id = Guid.NewGuid().ToString() });
        }
        
        if (saturation == -1)
        {
            Assert.IsFalse(await queue.IsSaturatedAsync());
            return;
        }
        
        Assert.AreEqual(saturation < itemCount, await queue.IsSaturatedAsync());   
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        await container.DeleteContainerAsync();
    }
}