using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ServiceLayerFunctions
{
    public static class GetAlerts
    {
        [FunctionName("GetAlerts")]
        public static async Task<IActionResult> RunGetAlerts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "alerts/")] HttpRequest req,
            [CosmosDB(databaseName: "%AlertDBName%",
                      collectionName: "%AlertCollectionName%",
                      ConnectionStringSetting = "AlertDBConnection",
                      SqlQuery = "select * from r")] IEnumerable<Alert> availableAlerts,
            ILogger log)
        {
            log.LogInformation("GetAlerts triggered by http request.");
            if (availableAlerts.ToList().Count == 0)
            {
                return new NoContentResult();
            }
            return new OkObjectResult(JsonConvert.SerializeObject(availableAlerts));
            
        }
    }
}
