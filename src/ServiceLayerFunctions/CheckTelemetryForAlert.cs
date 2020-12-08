using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace ServiceLayerFunctions
{
    public static class CheckTelemetryForAlert
    {
        public class Telemetry
        {
            [JsonProperty("speed")]
            public double Speed { get; set; }
        }

        //STEP 1: Define bindings
        [FunctionName("CheckTelemetryForAlert")]
        public static async Task Run([EventHubTrigger("%TelemetryHubName%", Connection = "TelemetryHubConn")] EventData[] events,
            [CosmosDB(databaseName: "%AlertDBName%",
                      collectionName: "%AlertCollectionName%",
                      ConnectionStringSetting = "AlertDBConnection",
                      SqlQuery = "select * from r")] IEnumerable<Alert> availableAlerts, 
            [SignalR(HubName = "%SignalrHubName%", ConnectionStringSetting = "SignalrHubConnection")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    //STEP 2: Read data and deserialize it to compare with thresholds in the next step
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    var telemetry = JsonConvert.DeserializeObject<Telemetry>(messageBody);
                    //STEP 3: Get only those alerts from the database that are below the current speed
                    var alert = availableAlerts.ToList().Find(alertDefinition => alertDefinition.AlertThreshold <= telemetry.Speed);

                    if (alert != null)
                    {
                        //STEP 4: Send a message via SignalR (Websocket) to our dashboard
                        var message = new
                        {
                            speedLimit = alert.AlertThreshold,
                            currentSpeed = telemetry.Speed
                        };
                        await signalRMessages.AddAsync(new SignalRMessage()
                        {
                            Target = "alerts",
                            Arguments = new object[] { JsonConvert.SerializeObject(message) }
                        });
                    }

                    await Task.Yield();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
