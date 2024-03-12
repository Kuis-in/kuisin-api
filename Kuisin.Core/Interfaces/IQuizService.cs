using Kuisin.Core.Enums;
using Kuisin.Core.Models;

namespace Kuisin.Core.Interfaces
{
    public interface IQuizService
    {
        Task<Quiz> GenerateQuizFromTranscriptAsync(string transcript, QuizGeneratorOptions? options = null);
    }
}
