using Kuisin.Core.Models;
using Riok.Mapperly.Abstractions;

namespace Kuisin.Core.Extensions
{
    [Mapper]
    public partial class Mapper
    {
        [MapProperty(nameof(CreateQuizRequest.NumberOfQuestions), nameof(@Job.QuizGeneratorOptions.NumberOfQuestions))]
        [MapProperty(nameof(CreateQuizRequest.QuizLanguage), nameof(@Job.QuizGeneratorOptions.Language))]
        [MapProperty(nameof(CreateQuizRequest.WordingStyle), nameof(@Job.QuizGeneratorOptions.WordingStyle))]
        public partial Job MapCreateJobRequestToJob(CreateQuizRequest job);
    }
}
