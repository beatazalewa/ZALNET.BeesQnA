using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ZALNET.BeesQnA.QueueReceiver
{
    public class GetDataFromQueue
    {
        [FunctionName("GetDataFromQueue")]
        public void Run([ServiceBusTrigger("beesqna-queue", Connection = "ConnectionString")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
