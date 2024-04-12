
using Microsoft.Azure.Cosmos;

namespace Adamijak.Cosmos.Configuration;

public class CosmosConfigurationOptions
{
    public required Container Container { get; init; }
    public required TimeSpan RefreshPeriod { get; init; }
    public required string Id { get; init; }
    public required PartitionKey PartitionKey { get; init; }
}