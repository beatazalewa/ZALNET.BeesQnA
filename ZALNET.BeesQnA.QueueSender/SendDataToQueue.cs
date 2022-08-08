using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace ZALNET.BeesQnA.QueueSender
{
    public static class SendDataToQueue
    {
        [FunctionName("SendDataToQueue")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            // Create a ServiceBusClient that will authenticate using a connection string
            string connectionString = "Endpoint=sb://zalnetbeesqna.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=HRoaeSHxSdGUKjuvkCdZn1ggWslGAm25nWumQS/lah8=";
            string queueName = "beesqna-queue";
            // since ServiceBusClient implements IAsyncDisposable we create it with "await using"
            await using var client = new ServiceBusClient(connectionString);

            //QueueClient client = new QueueClient("", "orders");

            string sourceType = req.Query["sourceType"];
            string userId = req.Query["userId"];
            string sourceKey = req.Query["sourceKey"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            sourceType = sourceType ?? data?.sourceType;
            userId = userId ?? data?.userId;
            sourceKey = sourceKey ?? data?.sourceKey;
            // create the sender
            ServiceBusSender sender = client.CreateSender(queueName);
            // create a message that we can send. UTF-8 encoding is used when providing a string.
            ServiceBusMessage message = new ServiceBusMessage($"SourceType: { sourceType }, UserId: { userId }, SourceKey: {sourceKey}");
            //ServiceBusMessage message = new ServiceBusMessage($"Hello Beata");
            await sender.SendMessageAsync(message);
            return new OkObjectResult(message);
        }
    }
}
