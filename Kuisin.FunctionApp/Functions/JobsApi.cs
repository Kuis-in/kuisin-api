using Kuisin.Core.Interfaces;
using Kuisin.Core.Models;
using Kuisin.Infrastructure.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Kuisin.FunctionApp.Functions
{
    public class JobsApi
    {
        private readonly ILogger<JobsApi> _logger;
        private readonly IJobService _jobService;
        private readonly IOptions<JsonOptions> _jsonOptions;

        public JobsApi(ILogger<JobsApi> logger, IJobService jobService, IOptions<JsonOptions> jsonOptions)
        {
            _logger = logger;
            _jobService = jobService;
            _jsonOptions = jsonOptions;
        }

        [Function(nameof(ScheduleJob))]
        public async Task<IActionResult> ScheduleJob([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "jobs")] [FromBody] CreateQuizRequest payload)
        {
            try
            {
                var job = await _jobService.ScheduleJobAsync(payload);
                if (job == null) return new NotFoundResult();
                return new OkObjectResult(job);
            }
            catch (InvalidOperationException ex)
            {
                return new BadRequestObjectResult(new GeneralResponse(ex.Message));
            }
        }

        [Function(nameof(GetCurrentUserJobs))]
        public async Task<IActionResult> GetCurrentUserJobs([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "jobs")] HttpRequest req)
        {
            var jobs = await _jobService.GetCurrentUserJobsAsync();
            return new OkObjectResult(jobs);
        }

        [Function(nameof(GetJobDetail))]
        public async Task<IActionResult> GetJobDetail([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "jobs/{jobId}")] HttpRequest req, string jobId)
        {
            var job = await _jobService.GetJobDetailAsync(jobId);
            return new OkObjectResult(job);
        }

        [Function(nameof(RetryJob))]
        public async Task<IActionResult> RetryJob([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "jobs/{jobId}/retry")] [FromBody] Job job, 
            string jobId, [DurableClient] DurableTaskClient durableClient)
        {
            _logger.LogInformation("Incoming job with id '{jobId}'. Job orchestration scheduled.", job.Id);
            await durableClient.ScheduleNewOrchestrationInstanceAsync(nameof(VideoToQuizOrchestration), job, new StartOrchestrationOptions { InstanceId = job.Id });
            return new NoContentResult();
        }

        [Function(nameof(HandleJobUpserted))]
        [SignalROutput(HubName = SignalR.JobLiveStatusHubName)]
        public async Task<List<object>> HandleJobUpserted([CosmosDBTrigger(
            databaseName: CosmosDb.DefaultDatabaseName,
            containerName: CosmosDb.JobsContainerName,
            Connection = Configuration.CosmosDBConnectionString,
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Job> jobs,
            [DurableClient] DurableTaskClient durableClient)
        {
            var signalRActions = new List<object>();
            foreach (var job in jobs)
            {
                var orchestration = await durableClient.GetInstanceAsync(job.Id);
                if (orchestration == null)
                {
                    _logger.LogInformation("Incoming job with id '{jobId}'. Job orchestration scheduled.", job.Id);
                    await durableClient.ScheduleNewOrchestrationInstanceAsync(nameof(VideoToQuizOrchestration), job, new StartOrchestrationOptions { InstanceId = job.Id });
                }
                else
                {
                    signalRActions.Add(new SignalRMessageAction("JobStatusUpdated")
                    {
                        GroupName = job.Id,
                        Arguments = new[] { job }
                    });
                }
            }
            return signalRActions;
        }
    }
}
