using EST.MIT.InvoiceImporter.Function.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

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
            //Log the memory stream length in kilobytes
            log.LogInformation($"[MainTrigger] Memory stream length: {memoryStream.Length / 1024} KB");
        }
    }
}