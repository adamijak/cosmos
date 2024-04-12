
using Microsoft.Extensions.Configuration;

namespace Adamijak.Cosmos.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddCosmosConfiguration(this IConfigurationBuilder builder, CosmosConfigurationOptions options)
    {
        builder.Add(new CosmosConfigurationSource(options));
        return builder;
    }
}