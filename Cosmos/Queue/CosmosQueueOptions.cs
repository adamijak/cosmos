using Microsoft.Azure.Cosmos;

namespace Adamijak.Cosmos.Queue;

public class CosmosQueueOptions<T> where T : QueueItem
{
    public required Container Container { get; set; }
    public Func<T, PartitionKey> PartitionKeySelector { get; set; } = i => new PartitionKey(i.Id);
    public int SaturationThreshold { get; set; }
}