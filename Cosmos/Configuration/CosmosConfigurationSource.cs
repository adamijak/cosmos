using Microsoft.Extensions.Configuration;

namespace Adamijak.Cosmos.Configuration;

public sealed class CosmosConfigurationSource(CosmosConfigurationOptions options) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) => new CosmosConfigurationProvider(options);
}