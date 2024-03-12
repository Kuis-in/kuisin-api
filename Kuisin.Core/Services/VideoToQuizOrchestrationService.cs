using Kuisin.Core.Constants;
using Kuisin.Core.Interfaces;
using Kuisin.Core.Models;
using Microsoft.Extensions.Logging;

namespace Kuisin.Core.Services
{
    internal class VideoToQuizOrchestrationService : IVideoToQuizOrchestrationService
    {
        private readonly ILogger<VideoToQuizOrchestrationService> _logger;
        private readonly IVideoService _videoService;
        private readonly ITranscriptionService _transcriptionService;
        private readonly IQuizService _quizService;
        private readonly IJobRepository _jobRepository;

        public VideoToQuizOrchestrationService(ILogger<VideoToQuizOrchestrationService> logger, IVideoService videoService, ITranscriptionService transcriptionService, IQuizService quizService, IJobRepository jobRepository)
        {
            _logger = logger;
            _videoService = videoService;
            _transcriptionService = transcriptionService;
            _quizService = quizService;
            _jobRepository = jobRepository;
        }

        private void Log(JobOrchestrationState orchestrationState, string message)
        {
            _logger.LogInformation("Job [{jobId}] => {message}", orchestrationState.Job.Id, message);
        }

        public JobOrchestrationState InitState(Job job)
        {
            return new JobOrchestrationState { Job = job };
        }

        public async Task<JobOrchestrationState> ExtractAudioFromVideoAsync(JobOrchestrationState orchestrationState)
        {
            var job = orchestrationState.Job;

            if (job.ResultTranscript == null)
            {
                orchestrationState.AudioFilePath = await _videoService.ExtractAudioFromYoutubeVideoAsync(job.VideoId, job.Id);
                Log(orchestrationState, string.Format("Audio extracted from video '{0}' and saved to '{1}'", job.VideoId, orchestrationState.AudioFilePath));
            }

            return orchestrationState;
        }

        public async Task<JobOrchestrationState> GetAudioTranscriptAsync(JobOrchestrationState orchestrationState)
        {
            if (orchestrationState.Job.ResultTranscript == null)
            {
                orchestrationState.Job.ResultTranscript = await _transcriptionService.GetAudioTranscriptionAsync(orchestrationState.AudioFilePath!);
                Log(orchestrationState, "Audio transcript generated");
            }
            return orchestrationState;
        }

        public async Task<JobOrchestrationState> GenerateQuizAsync(JobOrchestrationState orchestrationState)
        {
            if (orchestrationState.Job.ResultQuiz == null)
            {
                orchestrationState.Job.ResultQuiz = await _quizService.GenerateQuizFromTranscriptAsync(orchestrationState.Job.ResultTranscript!, orchestrationState.Job.QuizGeneratorOptions);
                orchestrationState.Job.Status = JobStatus.Done;
                Log(orchestrationState, "Quiz generated");
            }
            return orchestrationState;
        }

        public async Task<JobOrchestrationState> UpdateJobStatusAsync(JobOrchestrationState orchestrationState)
        {
            orchestrationState.Job.UtcUpdatedAt = DateTime.UtcNow;
            orchestrationState.Job = await _jobRepository.UpsertJobAsync(orchestrationState.Job);
            return orchestrationState;
        }
    }
}
