using Kuisin.Core.Extensions;
using Kuisin.Core.Interfaces;
using Kuisin.Core.Models;

namespace Kuisin.Core.Services
{
    internal class JobService : IJobService
    {
        private readonly Mapper _mapper;
        private readonly IVideoService _videoService;
        private readonly IJobRepository _repository;
        private readonly IIdentityService _identityService;

        public JobService(Mapper mapper, IVideoService videoService, IJobRepository repository, IIdentityService identityService)
        {
            _mapper = mapper;
            _videoService = videoService;
            _repository = repository;
            _identityService = identityService;
        }

        public async Task<List<Job>> GetCurrentUserJobsAsync()
        {
            var currentUserId = await _identityService.GetCurrentUserIdAsync();
            return await _repository.GetJobsByUserIdAsync(currentUserId);
        }

        public async Task<Job> GetJobDetailAsync(string jobId)
        {
            var currentUserId = await _identityService.GetCurrentUserIdAsync();
            return await _repository.GetJobDetailAsync(jobId, currentUserId);
        }

        public async Task<Job?> ScheduleJobAsync(CreateQuizRequest payload)
        {
            var currentUserId = await _identityService.GetCurrentUserIdAsync();
            var video = await _videoService.GetYoutubeVideoMetadataAsync(payload.VideoId);
            if (video == null) return null;

            if (video.DurationSeconds > 1800)
            {
                throw new InvalidOperationException("Durasi video tidak boleh lebih dari 30 menit");
            }

            var job = _mapper.MapCreateJobRequestToJob(payload);
            job.UserId = currentUserId;
            job.VideoTitle = video.Title;
            job.VideoThumbnailUrl = video.ThumbnailUrl;
            job.VideoSource = video.Source;

            return await _repository.UpsertJobAsync(job);
        }
    }
}
