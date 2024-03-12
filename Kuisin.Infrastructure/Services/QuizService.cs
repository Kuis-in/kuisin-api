using Kuisin.Core.Enums;
using Kuisin.Core.Interfaces;
using Kuisin.Core.Models;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Text.Json;

namespace Kuisin.Infrastructure.Services
{
    internal class QuizService : IQuizService
    {
        private readonly IOpenAIAPI _openAI;

        public QuizService(IOpenAIAPI openAI)
        {
            _openAI = openAI;
        }

        public async Task<Quiz> GenerateQuizFromTranscriptAsync(string transcript, QuizGeneratorOptions? options = null)
        {
            options ??= new QuizGeneratorOptions();

            var languageStr = options.Language switch
            {
                Language.EN => "English",
                Language.ID => "Indonesian",
                _ => throw new NotImplementedException()
            };

            var chatRequest = new ChatRequest
            {
                Model = Model.GPT4_Turbo,
                ResponseFormat = ChatRequest.ResponseFormats.JsonObject,
                Messages =
                [
                    new ChatMessage
                    {
                        Role = ChatMessageRole.System,
                        TextContent = 
                            $"Given a video transcript, generate a quiz based on it in {languageStr} which contains {options.NumberOfQuestions} questions, each with {options.NumberOfAnswers} unique answer choices (1 must be correct). " +
                            string.Format("Make it sound {0}. ", options.WordingStyle == QuizWordingStyle.Funny ? "funny with a little bit of slang words" : "formal and straightforward") +
                            "Order of the question MUST start from 0. " +
                            "Generate valid JSON which follows the following example: {\"questions\": [{\"order\": 0, \"questionText\": \"Question 1\", \"answers\": [{\"answerText\": \"Answer 1\", \"isValid\": true}]}]}"
                    },
                    new ChatMessage
                    {
                        Role = ChatMessageRole.User,
                        TextContent = transcript,
                    }
                ]
            };

            var results = await _openAI.Chat.CreateChatCompletionAsync(chatRequest);
            if (results == null) throw new InvalidOperationException("Failed to generate quiz");

            return JsonSerializer.Deserialize<Quiz>(results.Choices[0].Message.TextContent)!;
        }
    }
}
