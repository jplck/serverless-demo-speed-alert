using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Newtonsoft.Json;

namespace LoadSim
{
    class Program
    {
        private static string hubConnection = "Endpoint=sb://telemetryehns-serverless-demo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=0af6wN2PV8sXKZc1xyCyVgwEerCQZoXEKgil2xi7KQE=";
        private static string ehName = "telemetryhub";

        private static Random random;

        private static double simulatedSpeed = 0.0;

        static async Task Main(string[] args)
        {
            simulatedSpeed += GetRandom(-0.2, 0.2);
            
            await using (var client = new EventHubProducerClient(hubConnection, ehName))
            {
                using EventDataBatch eventBatch = await client.CreateBatchAsync();
                while (true)
                {
                    int i = 1;
                    while (i > 0)
                    {
                        var payload = new {
                            speed = simulatedSpeed
                        };

                        var data = new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
                        eventBatch.TryAdd(data);
                        i--;
                    }
                    await client.SendAsync(eventBatch);
                    await Task.Delay(1000);
                }
            }
        }

        private static double GetRandom(double min, double max)
        {
            return random.NextDouble() * (max - min) + min;
        }
    }
}
