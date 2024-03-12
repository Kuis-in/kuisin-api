using Kuisin.Core.Models;

namespace Kuisin.Core.Interfaces
{
    public interface IJobRepository
    {
        Task<Job> UpsertJobAsync(Job job);
        Task<List<Job>> GetJobsByUserIdAsync(string userId);
        Task<Job> GetJobDetailAsync(string jobId, string userId);
    }
}
