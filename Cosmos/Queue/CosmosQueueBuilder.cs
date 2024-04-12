using Microsoft.Azure.Cosmos;

namespace Adamijak.Cosmos.Queue;

public class CosmosQueueBuilder<T>(Container container)
    where T : QueueItem
{
    private CosmosQueueOptions<T> options = new()
    {
        Container = container,
    };
    
    public CosmosQueueBuilder<T> WithPartitionKey(Func<T, PartitionKey> partitionKeySelector )
    {
        options.PartitionKeySelector = partitionKeySelector;
        return this;
    }
    
    public CosmosQueue<T> Build()
    {
        return new CosmosQueue<T>(options);
    }
}