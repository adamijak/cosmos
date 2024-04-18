using Microsoft.Azure.Cosmos;

namespace Adamijak.Cosmos.Queue;

public class CosmosQueueBuilder<T>(Container container)
    where T : IQueueItem
{
    public static CosmosQueueBuilder<T> FromClient(CosmosClient client, string databaseId, string containerId)
    {
        return new CosmosQueueBuilder<T>(client.GetContainer(databaseId, containerId));
    }

    private readonly CosmosQueueOptions<T> options = new();
    
    public CosmosQueueBuilder<T> WithPartitionKey(Func<T, PartitionKey> partitionKeySelector )  
    {
        options.PartitionKeySelector = partitionKeySelector;
        return this;
    }
    
    public CosmosQueueBuilder<T> WithSaturationThreshold(int saturationThreshold)
    {
        options.SaturationThreshold = saturationThreshold;
        return this;
    }
    
    public CosmosQueue<T> Build()
    {
        return new CosmosQueue<T>(container, options);
    }
}