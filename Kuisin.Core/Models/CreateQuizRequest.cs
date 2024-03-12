using Kuisin.Core.Enums;
using System.Text.Json.Serialization;

namespace Kuisin.Core.Models
{
    public class CreateQuizRequest
    {
        [JsonPropertyName("videoId")]
        public required string VideoId { get; init; }

        [JsonPropertyName("numberOfQuestions")]
        public int NumberOfQuestions { get; init; } = 5;

        [JsonPropertyName("quizLanguage")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Language QuizLanguage { get; init; } = Language.ID;

        [JsonPropertyName("wordingStyle")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public QuizWordingStyle WordingStyle { get; init; } = QuizWordingStyle.Formal;
    }
}
