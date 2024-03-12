using Kuisin.Core.Extensions;
using Kuisin.Core.Interfaces;
using Kuisin.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kuisin.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddSingleton(new Mapper());
            services.AddSingleton<IJobService, JobService>();
            services.AddSingleton<IVideoToQuizOrchestrationService, VideoToQuizOrchestrationService>();
            return services;
        }
    }
}
