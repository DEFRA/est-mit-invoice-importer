using System;
using System.IO;
using System.Threading.Tasks;
using EST.MIT.InvoiceImporter.Function.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace EST.MIT.Importer.Function.Services
{
    public class Importer : IImporter
    {
        [FunctionName("MainTrigger")]
        public async Task QueueTrigger(
            [QueueTrigger("invoice-importer", Connection = "QueueConnectionString")] string importMessage,
            IBinder blobBinder,
            ILogger log)
        {
            log.LogInformation($"[MainTrigger] Recieved message: {importMessage} at {DateTime.UtcNow.ToLongTimeString()}");
            BlobService blobService = new();
            Stream memoryStream = await blobService.ReadBLOBIntoStream(importMessage, log, blobBinder);
        }
    }
}