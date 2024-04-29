using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.Azure.Cosmos;

namespace Adamijak.CosmosBenchmark;

[SimpleJob(RunStrategy.ColdStart, iterationCount: 5)]
public class ConcurrentInsert
{
    [Params(10_000)]
    public int ItemCount { get; set; }
    [Params(1_000)]
    public int ItemDataSize { get; set; }
    
    [Params(100, 1_000, int.MaxValue)]
    public int TaskCount { get; set; }
    
    private Database? database;
    private Container? container;
    private Item[] items;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("COSMOS_CS"), new CosmosClientOptions()
        {
            AllowBulkExecution = true,
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        });
        database = cosmosClient.CreateDatabaseIfNotExistsAsync(Default.DatabaseId).Result;

        items = new Item[ItemCount];
        for (var i = 0; i < ItemCount; i++)
        {
            items[i] = new Item(ItemDataSize);
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        if (database is not null)
        {
            database.DeleteAsync().Wait();
        }
    }

    [IterationSetup]
    public void IterationSetup()
    {
        container = database!.CreateContainerIfNotExistsAsync(Guid.NewGuid().ToString(), "/id").Result;
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        if (container is not null)
        {
            container.DeleteContainerAsync().Wait();
        }
    }

    [Benchmark]
    public async Task ChunkInsert()
    {
        foreach (var chunk in items.Chunk(TaskCount))
        {
            var tasks = new List<Task>();
            foreach (var item in chunk)
            {
                tasks.Add(container.CreateItemAsync(item, new PartitionKey(item.Id)));
            }
            await Task.WhenAll(tasks);
        }
    }
    
    [Benchmark]
    public async Task SetInsert()
    {
        var tasks = new HashSet<Task>();
        foreach (var item in items)
        {
            if (TaskCount <= tasks.Count)
            {
                var task = await Task.WhenAny(tasks);
                tasks.Remove(task);
            }
            tasks.Add(container.CreateItemAsync(item, new PartitionKey(item.Id)));
        }

        await Task.WhenAll(tasks);
    }
}