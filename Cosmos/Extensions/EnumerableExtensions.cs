namespace Adamijak.Cosmos.Extensions;

public static class EnumerableExtensions
{
    public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action, int itemsConcurrent = 1, CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(itemsConcurrent, 1);

        var tasks = new HashSet<Task>();
        foreach (var item in source)
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