using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceLayerFunctions;

namespace Serverless.Demo
{
    public static class ServiceLayer
    {
        [FunctionName("CreateAlert")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "alert/")] HttpRequest req,
            [CosmosDB(databaseName: "%AlertDBName%",
                      collectionName: "%AlertCollectionName%",
                      CreateIfNotExists = true,
                      PartitionKey = "/id",
                      ConnectionStringSetting = "AlertDBConnection")] IAsyncCollector<Alert> alerts,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Alert serviceAlert = JsonConvert.DeserializeObject<Alert>(requestBody);

            serviceAlert.Id = Guid.NewGuid().ToString();

            await alerts.AddAsync(serviceAlert);

            log.LogInformation("Successfully created alert!");

            return new OkResult();
        }
    }
}
