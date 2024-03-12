using Kuisin.Core.Constants;
using Kuisin.Core.Enums;
using System.Text.Json.Serialization;

namespace Kuisin.Core.Models
{
    public class Job
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("videoId")]
        public required string VideoId { get; set; }

        [JsonPropertyName("videoTitle")]
        public string VideoTitle { get; set; } = string.Empty;

        [JsonPropertyName("videoThumbnailUrl")]
        public string VideoThumbnailUrl { get; set; } = string.Empty;

        [JsonPropertyName("videoSource")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public VideoSource VideoSource { get; set; }

        [JsonPropertyName("quizGeneratorOptions")]
        public QuizGeneratorOptions QuizGeneratorOptions { get; set; } = new QuizGeneratorOptions();

        [JsonPropertyName("status")]
        public string Status { get; set; } = JobStatus.Outstanding;

        [JsonPropertyName("resultTranscript")]
        public string? ResultTranscript { get; set; }

        [JsonPropertyName("resultQuiz")]
        public Quiz? ResultQuiz { get; set; }

        [JsonPropertyName("utcCreatedAt")]
        public DateTime UtcCreatedAt { get; init; } = DateTime.UtcNow;

        [JsonPropertyName("utcUpdatedAt")]
        public DateTime UtcUpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
