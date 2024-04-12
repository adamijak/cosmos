using System.Text.Json.Serialization;

namespace Adamijak.Cosmos.Queue;

public class QueueItem
{
    public string? Id { get; set; }
    
    [JsonPropertyName("_ts")]
    public long Timestamp { get; set; }
}