using Kuisin.Core.Interfaces;
using Kuisin.Infrastructure.Constants;
using Kuisin.Infrastructure.Interfaces;
using Kuisin.Infrastructure.Repositories;
using Kuisin.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI_API;

namespace Kuisin.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IOpenAIAPI>(new OpenAIAPI(configuration[Configuration.OpenAIApiKey]));
            services.AddSingleton<IFunctionContextAccessor, FunctionContextAccessor>();
            services.AddSingleton<IIdentityService, IdentityService>();
            services.AddSingleton<IJobRepository, JobRepository>();
            services.AddSingleton<IVideoService, VideoService>();
            services.AddSingleton<ITranscriptionService, TranscriptionService>();
            services.AddSingleton<IQuizService, QuizService>();
            return services;
        }
    }
}
