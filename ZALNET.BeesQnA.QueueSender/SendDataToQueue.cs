using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ZALNET.BeesQnA.QueueSender
{
    public static class SendDataToQueue
    {
        [FunctionName("SendDataToQueue")]
        [return: ServiceBus("az-queue")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            // Create a ServiceBusClient that will authenticate using a connection string
            string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            string queueName = Environment.GetEnvironmentVariable("QueueName");
            // since ServiceBusClient implements IAsyncDisposable we create it with "await using"
            await using var client = new ServiceBusClient(connectionString);

            //QueueClient client = new QueueClient("", "orders");

            string beesType = req.Query["BeesType"];
            string beehiveId = req.Query["BeehiveId"];
            string beesDescription = req.Query["BeesDescription"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            beesType = beesType ?? data?.beesType;
            beehiveId = beehiveId ?? data?.beehiveId;
            beesDescription = beesDescription ?? data?.beesDescription;
            // create the sender
            ServiceBusSender sender = client.CreateSender(queueName);
            // create a message that we can send. UTF-8 encoding is used when providing a string.
            ServiceBusMessage message = new ServiceBusMessage($"Bees type: { beesType }, beehive identity: { beehiveId }, bees description: {beesDescription}");
            //ServiceBusMessage message = new ServiceBusMessage($"Hello Beata");
            await sender.SendMessageAsync(message);
            return new OkObjectResult(message);
        }
    }
}
