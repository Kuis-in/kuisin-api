using Kuisin.Core.Interfaces;
using Kuisin.Core.Models;
using Kuisin.Infrastructure.Constants;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Kuisin.Infrastructure.Repositories
{
    internal class JobRepository : BaseCosmosRepository, IJobRepository
    {
        public JobRepository(IConfiguration configuration, IOptions<JsonOptions> jsonOptions) : base(configuration, jsonOptions)
        {
        }

        public async Task<List<Job>> GetJobsByUserIdAsync(string userId)
        {
            var jobs = new List<Job>();
            var container = Client.GetContainer(CosmosDb.DefaultDatabaseName, CosmosDb.JobsContainerName);

            using var feed = container.GetItemQueryIterator<Job>(
                queryDefinition: new QueryDefinition(query: "SELECT * FROM c WHERE c.userId = @userId ORDER BY c.utcUpdatedAt DESC")
                    .WithParameter("@userId", userId)
            );

            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync();
                jobs.AddRange(response);
            }

            return jobs;
        }

        public async Task<Job> GetJobDetailAsync(string jobId, string userId)
        {
            var container = Client.GetContainer(CosmosDb.DefaultDatabaseName, CosmosDb.JobsContainerName);

            var paritionKey = new PartitionKeyBuilder()
                .Add(userId)
                .Add(jobId)
                .Build();

            var result = await container.ReadItemAsync<Job>(jobId, paritionKey);
            return result.Resource;
        }

        public async Task<Job> UpsertJobAsync(Job job)
        {
            var container = Client.GetContainer(CosmosDb.DefaultDatabaseName, CosmosDb.JobsContainerName);
            var item = await container.UpsertItemAsync(job);
            return item.Resource;
        }
    }
}
