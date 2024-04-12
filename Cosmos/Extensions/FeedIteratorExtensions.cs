using System.Runtime.CompilerServices;
using Microsoft.Azure.Cosmos;

namespace Adamijak.Cosmos.Extensions;

public static class FeedIteratorExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this FeedIterator<T> iterator,
        [EnumeratorCancellation] CancellationToken cancelToken = default)
    {
        while (iterator.HasMoreResults)
        {
            var values = await iterator.ReadNextAsync(cancelToken);
            foreach (var value in values)
            {
                yield return value;
            }
        }
    }
}