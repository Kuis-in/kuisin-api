namespace Kuisin.Infrastructure.Constants
{
    public static class Configuration
    {
        public const string ApplicationName = "AppSettings:ApplicationName";
        public const string CosmosDBConnectionString = "CosmosDB:ConnectionString";
        public const string OpenAIApiKey = "OpenAI:ApiKey";
        public const string JobRetryMaxNumberOfAttempts = "JobRetryPolicy:MaxNumberOfAttempts";
        public const string JobRetryFirstRetryIntervalSeconds = "JobRetryPolicy:FirstRetryIntervalSeconds";
        public const string SignalRConnectionString = "AzureSignalRConnectionString";
        public const string YoutubeApiKey = "YouTube:ApiKey";
    }
}
