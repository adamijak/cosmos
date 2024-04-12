using Microsoft.Extensions.Configuration.Json;

namespace Adamijak.Cosmos.Configuration;

public sealed class CosmosConfigurationProvider : JsonStreamConfigurationProvider, IDisposable
{
    private readonly CosmosConfigurationOptions options;
    private readonly Timer timer;
    private string? etag;

    public CosmosConfigurationProvider(CosmosConfigurationOptions options) : base(new())
    {
        this.options = options;
        timer = new Timer((s) =>
        {
            Load();
            OnReload();
        }, null, options.RefreshPeriod, options.RefreshPeriod);
    }
    
    public override void Load()
    {
        var response = Task.Run(() => options.Container.ReadItemStreamAsync(options.Id, options.PartitionKey)).Result;
        if (!response.IsSuccessStatusCode || (etag is not null && etag == response.Headers["ETag"]))
        {
            return;
        }
        etag = response.Headers["ETag"];
        Load(response.Content);
        Data.Remove("id");
        Data.Remove("_rid");
        Data.Remove("_self");
        Data.Remove("_etag");
        Data.Remove("_attachments");
        Data.Remove("_ts");
    }

    public void Dispose()
    {
        timer.Dispose();
    }
}