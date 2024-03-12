using Kuisin.Core.Models;

namespace Kuisin.Core.Interfaces
{
    public interface IJobService
    {
        Task<Job?> ScheduleJobAsync(CreateQuizRequest payload);
        Task<List<Job>> GetCurrentUserJobsAsync();
        Task<Job> GetJobDetailAsync(string jobId);
    }
}
