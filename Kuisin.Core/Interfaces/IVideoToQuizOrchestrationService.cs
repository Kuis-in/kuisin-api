using Kuisin.Core.Models;

namespace Kuisin.Core.Interfaces
{
    public interface IVideoToQuizOrchestrationService
    {
        JobOrchestrationState InitState(Job job);
        Task<JobOrchestrationState> ExtractAudioFromVideoAsync(JobOrchestrationState orchestrationState);
        Task<JobOrchestrationState> GetAudioTranscriptAsync(JobOrchestrationState orchestrationState);
        Task<JobOrchestrationState> GenerateQuizAsync(JobOrchestrationState orchestrationState);
        Task<JobOrchestrationState> UpdateJobStatusAsync(JobOrchestrationState orchestrationState);
    }
}
