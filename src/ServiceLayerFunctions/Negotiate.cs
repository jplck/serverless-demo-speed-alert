using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Serverless.Demo
{

    public static class Negotiate
    {
        [FunctionName("Negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "telemetry/negotiate/")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "%SignalrHubName%", ConnectionStringSetting = "SignalrHubConnection")] SignalRConnectionInfo info,
            ILogger log)
        {
            return info;
        }

    }
}