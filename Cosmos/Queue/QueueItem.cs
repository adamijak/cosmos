using Newtonsoft.Json;

namespace Adamijak.Cosmos.Queue;

public class QueueItem: IQueueItem
{
    public string? Id { get; set; }
    [JsonProperty("_rid")]
    public string ResourceId { get; internal set; } = null!;
    [JsonProperty("_self")]
    public string Self { get; internal set; } = null!;
    [JsonProperty("_etag")]
    public string ETag { get; internal set; } = null!;
    [JsonProperty("_attachments")]
    public string Attachments { get; internal set; } = null!;
    [JsonProperty("_ts")]
    public long Timestamp { get; internal set; }
}