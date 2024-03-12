using Kuisin.Infrastructure.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Kuisin.FunctionApp.Functions
{
    public class JobLiveStatusHub
    {
        [Function("JobLiveStatusHub_Negotiate")]
        public static IActionResult Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "jobs/live-status-ws/negotiate")] HttpRequest req,
            [SignalRConnectionInfoInput(HubName = SignalR.JobLiveStatusHubName, UserId = "{headers.x-ms-client-principal-id}")] SignalRConnectionInfo connectionInfo)
        {
            return new OkObjectResult(connectionInfo);
        }

        [Function("JobLiveStatusHub_ListenOnJob")]
        [SignalROutput(HubName = SignalR.JobLiveStatusHubName)]
        public static SignalRGroupAction ListenOnJob([SignalRTrigger(SignalR.JobLiveStatusHubName, "messages", "ListenOnJob", "jobId")] SignalRInvocationContext context, string jobId)
        {
            return new SignalRGroupAction(SignalRGroupActionType.Add)
            {
                ConnectionId = context.ConnectionId,
                GroupName = jobId,
            };
        }
    }
}