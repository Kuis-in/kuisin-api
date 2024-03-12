using Kuisin.Core.Constants;
using Kuisin.Core.Interfaces;
using Kuisin.Core.Models;
using Kuisin.Infrastructure.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Configuration;

namespace Kuisin.FunctionApp.Functions
{
    public class VideoToQuizOrchestration
    {
        private const string InitStateActivity = $"{nameof(VideoToQuizOrchestration)}_InitState";
        private const string ExtractAudioFromVideoActivity = $"{nameof(VideoToQuizOrchestration)}_ExtractAudioFromVideo";
        private const string GetAudioTranscriptActivity = $"{nameof(VideoToQuizOrchestration)}_GetAudioTranscript";
        private const string GenerateQuizActivity = $"{nameof(VideoToQuizOrchestration)}_GenerateQuiz";
        private const string UpdateJobStatusActivity = $"{nameof(VideoToQuizOrchestration)}_UpdateJobStatus";

        private readonly IConfiguration _configuration;
        private readonly IVideoToQuizOrchestrationService _orchestrationService;
        private readonly TaskOptions _taskOptions;

        public VideoToQuizOrchestration(IConfiguration configuration, IVideoToQuizOrchestrationService orchestrationService)
        {
            _configuration = configuration;
            _orchestrationService = orchestrationService;

            // Initialize retry policy
            var retryPolicy = new RetryPolicy(
                _configuration.GetValue<int>(Configuration.JobRetryMaxNumberOfAttempts),
                TimeSpan.FromSeconds(_configuration.GetValue<int>(Configuration.JobRetryFirstRetryIntervalSeconds)));

            _taskOptions = new TaskOptions(new TaskRetryOptions(retryPolicy));
        }

        [Function(nameof(VideoToQuizOrchestration))]
        public async Task<JobOrchestrationState> Run([OrchestrationTrigger] TaskOrchestrationContext context, Job input)
        {
            var state = await context.CallActivityAsync<JobOrchestrationState>(InitStateActivity, input);
            try
            {
                state = await context.CallActivityAsync<JobOrchestrationState>(ExtractAudioFromVideoActivity, state, _taskOptions);
                state = await context.CallActivityAsync<JobOrchestrationState>(GetAudioTranscriptActivity, state, _taskOptions);
                state = await context.CallActivityAsync<JobOrchestrationState>(UpdateJobStatusActivity, state, _taskOptions);
                state = await context.CallActivityAsync<JobOrchestrationState>(GenerateQuizActivity, state, _taskOptions);
                state = await context.CallActivityAsync<JobOrchestrationState>(UpdateJobStatusActivity, state, _taskOptions);
            }
            catch (Exception)
            {
                state.Job.Status = JobStatus.Failed;
                await context.CallActivityAsync<JobOrchestrationState>(UpdateJobStatusActivity, state, _taskOptions);
                throw;
            }
            return state;
        }

        [Function(InitStateActivity)]
        public JobOrchestrationState InitState([ActivityTrigger] Job input)
        {
            return _orchestrationService.InitState(input);
        }

        [Function(ExtractAudioFromVideoActivity)]
        public Task<JobOrchestrationState> ExtractAudioFromVideo([ActivityTrigger] JobOrchestrationState state)
        {
            return _orchestrationService.ExtractAudioFromVideoAsync(state);
        }

        [Function(GetAudioTranscriptActivity)]
        public Task<JobOrchestrationState> GetAudioTranscript([ActivityTrigger] JobOrchestrationState state)
        {
            return _orchestrationService.GetAudioTranscriptAsync(state);
        }

        [Function(GenerateQuizActivity)]
        public Task<JobOrchestrationState> GenerateQuiz([ActivityTrigger] JobOrchestrationState state)
        {
            return _orchestrationService.GenerateQuizAsync(state);
        }

        [Function(UpdateJobStatusActivity)]
        public Task<JobOrchestrationState> UpdateJobStatus([ActivityTrigger] JobOrchestrationState state)
        {
            return _orchestrationService.UpdateJobStatusAsync(state);
        }
    }
}
