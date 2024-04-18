namespace Adamijak.Cosmos.Extensions;

public static class AsyncEnumerableExtensions
{
    public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> source, Func<T, Task> action, int itemsConcurrent = 1, CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(itemsConcurrent, 1);

        var tasks = new HashSet<Task>();
        await foreach (var item in source.WithCancellation(ct))
        {
            if (itemsConcurrent <= tasks.Count)
            {
                var task = await Task.WhenAny(tasks);
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
                tasks.Remove(task);
            }
            
            tasks.Add(action(item));
        }
        await Task.WhenAll(tasks);
    }
}