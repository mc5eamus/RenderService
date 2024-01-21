using System.Text.Json.Serialization;

namespace RenderDomain.Model
{
    /// <summary>
    /// Processing task status
    /// </summary>
    public enum RenderTaskStatus
    {
        Queued,
        Processing,
        Complete,
        Failed
    }

    /// <summary>
    /// Describes a simple render task
    /// </summary>
    public class RenderTask
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("source")]
        public string? Source { get; set; }
        [JsonPropertyName("status")]
        public RenderTaskStatus Status { get; set; }

        [JsonPropertyName("processingHost")]
        public string? ProcessingHost { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    }
}
