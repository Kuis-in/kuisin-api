using System.Text.Json.Serialization;

namespace Kuisin.Core.Models
{
    public class Quiz
    {
        [JsonPropertyName("questions")]
        public required List<QuizQuestion> Questions { get; init; }
    }

    public class QuizQuestion
    {
        [JsonPropertyName("order")]
        public required int Order { get; init; }

        [JsonPropertyName("questionText")]
        public required string QuestionText { get; init; }

        [JsonPropertyName("answers")]
        public required List<QuizAnswer> Answers { get; init; }
    }

    public class QuizAnswer
    {
        [JsonPropertyName("answerText")]
        public required string AnswerText { get; init; }

        [JsonPropertyName("isValid")]
        public required bool IsValid { get; init; }
    }
}
