using Kuisin.Core;
using Kuisin.Core.Models;
using Kuisin.FunctionApp.Middlewares;
using Kuisin.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection;
using ThrottlingTroll;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
    {
        builder.UseMiddleware<FunctionContextAccessorMiddleware>();
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
        builder.UseThrottlingTroll(options =>
        {
            options.Config = new ThrottlingTrollConfig
            {
                Rules =
                [
                    new ThrottlingTrollRule
                    {
                        UriPattern = "/jobs",
                        Method = "POST",
                        LimitMethod = new SlidingWindowRateLimitMethod
                        {
                            PermitLimit = 5,
                            IntervalInSeconds = 60,
                            NumOfBuckets = 2
                        },
                        IdentityIdExtractor = (request) =>
                        {
                            return ((IIncomingHttpRequestProxy)request).Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"];
                        }
                    }
                ]
            };
        });
    })
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        if (Debugger.IsAttached)
        {
            configBuilder.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
        }
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.ConfigureHttpClientDefaults(config =>
        {
            config.ConfigureHttpClient((provider, client) =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });
        });

        services.AddSingleton(new AppConfig
        {
            ApplicationRootPath = hostContext.HostingEnvironment.ContentRootPath
        });

        services.AddCore();
        services.AddInfrastructure(hostContext.Configuration);
    })
    .Build();

host.Run();
