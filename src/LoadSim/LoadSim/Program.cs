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
        private static string hubConnection = Environment.GetEnvironmentVariable("ConnectionString");
        private static string ehName = Environment.GetEnvironmentVariable("EHName");

        private static Random random = new Random();

        private static double simulatedSpeed = 50;

        static async Task Main(string[] args)
        {
            await using (var client = new EventHubProducerClient(hubConnection, ehName))
            {
                using EventDataBatch eventBatch = await client.CreateBatchAsync();
                while (true)
                {
                    int i = 1;
                    while (i > 0)
                    {
                        simulatedSpeed += GetRandom(-0.2, 0.2);
                        Console.WriteLine(simulatedSpeed);
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
