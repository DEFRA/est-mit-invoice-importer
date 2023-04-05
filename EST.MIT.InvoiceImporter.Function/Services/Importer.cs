using Azure.Identity;
using Azure.Storage.Blobs;
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

            var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("StorageConnectionString"));

            BlobService blobService = new();
            Stream memoryStream = await blobService.ReadBLOBIntoStream(importMessage, log, blobBinder);
            if (memoryStream != null)
            {
                log.LogInformation($"[MainTrigger] file read into memory stream, filelength: {memoryStream.Length / 1024} KB");
                log.LogInformation($"Moving file to archive: {importMessage} at {DateTime.UtcNow.ToLongTimeString()}");
                if(await BlobService.MoveFileToArchive(blobService.GetFileName(), log, blobServiceClient))
                {
                    log.LogInformation($"File moved to archive: {importMessage} at {DateTime.UtcNow.ToLongTimeString()}");
                }
            }
        }
    }
}