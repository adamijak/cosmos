using Adamijak.Cosmos.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Adamijak.Cosmos.Queue;

public class CosmosQueue<T>(Container container, CosmosQueueOptions<T> options) where T : IQueueItem
{
    public async Task<int> CountAsync() => 
        await container.GetItemLinqQueryable<object>().CountAsync();

    public async Task<bool> IsSaturatedAsync() => 
        options.SaturationThreshold != -1
        && options.SaturationThreshold < await CountAsync();
    
    public Task<ItemResponse<T>> EnqueueAsync(T item) =>
        container.CreateItemAsync(item, options.PartitionKeySelector(item));
    
    public Task<ItemResponse<T>> DeleteAsync(T item) =>
        container.DeleteItemAsync<T>(item.Id, options.PartitionKeySelector(item));

    public async Task<TransactionalBatchOperationResult<T>> DequeueAsync(T item)
    {
        var response = await container.CreateTransactionalBatch(options.PartitionKeySelector(item))
            .ReadItem(item.Id)
            .DeleteItem(item.Id)
            .ExecuteAsync();
        var r0 = response.GetOperationResultAtIndex<T>(0);
        return r0;
    }

    public IAsyncEnumerable<T> ToAsyncEnumerable(CancellationToken ct = default) => ToAsyncEnumerable(i => i, ct);
    
    public IAsyncEnumerable<T> ToAsyncEnumerable(Func<IOrderedQueryable<T>, IQueryable<T>> query, CancellationToken ct = default)
    {
        using var iterator = query(container.GetItemLinqQueryable<T>())
            .ToFeedIterator();
        return iterator.ToAsyncEnumerable(ct);
    }
}