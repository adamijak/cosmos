using Microsoft.Azure.Cosmos;

namespace Adamijak.Cosmos.Queue;

public class CosmosQueueOptions<T> where T : IQueueItem
{
    public Func<T, PartitionKey> PartitionKeySelector { get; set; } = i => new PartitionKey(i.Id);
    public int SaturationThreshold { get; set; } = -1;
}