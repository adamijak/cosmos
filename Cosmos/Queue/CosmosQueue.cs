using Adamijak.Cosmos.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Adamijak.Cosmos.Queue;

public class CosmosQueue<T>(CosmosQueueOptions<T> options) where T : QueueItem
{
    private readonly Container container = options.Container;

    public async Task<bool> IsSaturatedAsync() => options.SaturationThreshold <= await container.GetItemLinqQueryable<object>().CountAsync();

    public const int DefaultChunkSize = 1_000;

    public async Task EnqueueItemsAsync(T[] items)
    {
        if (items.Length == 0)
        {
            return;
        }

        foreach (var chunk in items.Chunk(DefaultChunkSize))
        {
            var tasks = new List<Task>(DefaultChunkSize);
            foreach (var item in chunk)
            {
                tasks.Add(EnqueueItemAsync(item));
            }

            await Task.WhenAll(tasks);
        }
    }

    public async Task EnqueueItemAsync(T item)
    {
        await container.CreateItemAsync(item, options.PartitionKeySelector(item));
    }

    public Task DeleteItemAsync(T item) =>
        container.DeleteItemAsync<object>(item.Id, options.PartitionKeySelector(item));

    public IAsyncEnumerable<T> ToAsyncEnumerable() => ToAsyncEnumerable(i => i);
    
    public IAsyncEnumerable<T> ToAsyncEnumerable(Func<IOrderedQueryable<T>, IQueryable<T>> query, CancellationToken ct = default)
    {
        using var iterator = query(container.GetItemLinqQueryable<T>())
                .ToFeedIterator();
        return iterator.ToAsyncEnumerable(ct);
    }
}