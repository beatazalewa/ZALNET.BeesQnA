using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ZALNET.BeesQnA.QueueReceiver
{
    public static class GetDataFromQueue
    {
        [FunctionName(nameof(GetDataFromQueue))]
        public static async Task Run(
            [ServiceBusTrigger("beesqna-queue", Connection = "ServiceBusConnectionString")] string payload,
            string messageId,
            [Blob("messages/{messageId}.txt", FileAccess.Write, Connection = "StorageAccountConnectionString")] Stream output)
        {
            await output.WriteAsync(Encoding.UTF8.GetBytes(payload));
        }
    }
}
