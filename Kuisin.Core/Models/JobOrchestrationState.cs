using System.Text.Json.Serialization;

namespace Kuisin.Core.Models
{
    public class JobOrchestrationState
    {
        [JsonPropertyName("jobDetail")]
        public required Job Job { get; set; }

        [JsonPropertyName("audioFilePath")]
        public string? AudioFilePath { get; set; }
    }
}
