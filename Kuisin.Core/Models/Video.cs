using Kuisin.Core.Enums;
using System.Text.Json.Serialization;

namespace Kuisin.Core.Models
{
    public class Video
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("description")]
        public required string Description { get; set; }

        [JsonPropertyName("thumbnailUrl")]
        public required string ThumbnailUrl { get; set; }

        [JsonPropertyName("videoUrl")]
        public required string VideoUrl { get; set; }

        [JsonPropertyName("source")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required VideoSource Source { get; set; }
    }
}
