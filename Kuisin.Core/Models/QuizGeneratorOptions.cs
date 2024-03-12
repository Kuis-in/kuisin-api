using Kuisin.Core.Enums;
using System.Text.Json.Serialization;

namespace Kuisin.Core.Models
{
    public class QuizGeneratorOptions
    {
        [JsonPropertyName("numberOfQuestions")]
        public int NumberOfQuestions { get; set; } = 5;

        [JsonPropertyName("numberOfAnswers")]
        public int NumberOfAnswers { get; set; } = 5;

        [JsonPropertyName("language")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Language Language { get; set; } = Language.ID;

        [JsonPropertyName("wordingStyle")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public QuizWordingStyle WordingStyle { get; set; } = QuizWordingStyle.Formal;
    }
}
