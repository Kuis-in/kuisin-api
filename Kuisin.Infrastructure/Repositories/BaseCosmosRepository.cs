using Kuisin.Infrastructure.Constants;
using Kuisin.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Kuisin.Infrastructure.Repositories
{
    internal class BaseCosmosRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<JsonOptions> _jsonOptions;

        protected readonly CosmosClient Client;

        public BaseCosmosRepository(IConfiguration configuration, IOptions<JsonOptions> jsonOptions)
        {
            _configuration = configuration;
            _jsonOptions = jsonOptions;

            // Initialize singleton CosmosClient
            Client = new(_configuration[Configuration.CosmosDBConnectionString], new CosmosClientOptions
            {
                Serializer = new CosmosJsonTextSerializer(_jsonOptions.Value.SerializerOptions)
            });
        }
    }
}
